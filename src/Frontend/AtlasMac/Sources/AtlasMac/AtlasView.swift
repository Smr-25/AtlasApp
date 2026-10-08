import SwiftUI
import UniformTypeIdentifiers

struct AtlasView: View {
    @ObservedObject var model: Workbench
    @FocusState private var searchFocused: Bool
    @State private var showRecipes = false
    @State private var dropped: [URL] = []

    var body: some View {
        HStack(spacing: 0) {
            sidebar
            Divider()
            VStack(spacing: 0) {
                toolbar
                Divider().opacity(0.5)
                if let tool = model.selectedTool { ToolWorkspace(model: model, tool: tool) }
                else if model.section == "Applications" { ContainerView(model: model.containers) }
                else if model.section == "Snippets" { SnippetView(model: model) }
                else if model.section == "Settings" { AppearanceSettings(model: model) }
                else { library }
            }.frame(maxWidth: .infinity, maxHeight: .infinity).background(AtlasStyle.background)
        }
        .tint(AtlasStyle.accent)
        .frame(minWidth: 1040, minHeight: 700)
        .sheet(isPresented: $model.showQueue) { JobQueueView(model: model).frame(width: 640, height: 470) }
        .sheet(isPresented: $showRecipes) { recipes.frame(width: 530, height: 460) }
        .alert("Atlas", isPresented: Binding(get: { model.notice != nil }, set: { if !$0 { model.notice = nil } })) {
            Button("OK") { model.notice = nil }
        } message: { Text(model.notice ?? "") }
    }

    private var sidebar: some View {
        VStack(alignment: .leading, spacing: 6) {
            AtlasWordmark().padding(.horizontal, 12).padding(.top, 30).padding(.bottom, 23)
            nav("All tools", icon: "square.grid.2x2")
            nav("Favorites", icon: "star")
            nav("Recent", icon: "clock.arrow.circlepath")
            Eyebrow(text: "Collections").padding(.leading, 13).padding(.top, 25).padding(.bottom, 8)
            ForEach(["Files", "Images", "PDF", "Text", "Technical"], id: \.self) { category in nav(category, icon: AtlasStyle.icon(category)) }
            Divider().padding(.vertical, 12)
            nav("Applications", icon: "shippingbox")
            nav("Snippets", icon: "text.bubble")
            Spacer(minLength: 12)
            nav("Settings", icon: "gearshape")
            Button { showRecipes = true } label: {
                Label("Saved recipes", systemImage: "slider.horizontal.3").font(.system(size: 12, weight: .medium)).padding(12).frame(maxWidth: .infinity, alignment: .leading).contentShape(Rectangle())
            }.buttonStyle(.plain)
            HStack(spacing: 9) {
                Circle().fill(.green).frame(width: 5, height: 5)
                VStack(alignment: .leading, spacing: 3) {
                    Text("Your Mac. Your tools.").font(.system(size: 11, weight: .semibold))
                    Text("Atlas 1.0").font(.system(size: 10)).foregroundStyle(.secondary)
                }
            }.padding(14).frame(maxWidth: .infinity, alignment: .leading).atlasPanel().padding(.bottom, 18)
        }.padding(.horizontal, 14).frame(width: 202).background(AtlasStyle.background)
    }

    private func nav(_ title: String, icon: String) -> some View {
        let active = model.selectedID == nil && model.section == title
        return Button { model.library(title); dropped = [] } label: {
            HStack(spacing: 11) {
                Image(systemName: icon).font(.system(size: 14)).frame(width: 19)
                Text(title).font(.system(size: 12, weight: active ? .semibold : .medium))
                Spacer()
                if ["Files", "Images", "PDF", "Text", "Technical"].contains(title) {
                    Text("\(toolCatalog.filter { $0.category == title }.count)").font(.system(size: 10, design: .monospaced)).foregroundStyle(.secondary)
                }
            }.padding(.horizontal, 12).frame(height: 36).contentShape(Rectangle())
                .foregroundStyle(active ? AtlasStyle.accent : Color.primary.opacity(0.78))
                .background(active ? AtlasStyle.accent.opacity(0.10) : .clear, in: RoundedRectangle(cornerRadius: 10))
        }.buttonStyle(.plain)
    }

    private var toolbar: some View {
        HStack(spacing: 12) {
            if model.selectedTool != nil {
                Button { model.library() } label: { Image(systemName: "chevron.left") }.buttonStyle(.plain).help("Back to tools")
                Text("Workbench").font(.system(size: 12, weight: .medium)).foregroundStyle(.secondary)
            } else { Text(model.section).font(.system(size: 12, weight: .semibold)) }
            Spacer()
            HStack(spacing: 7) {
                Image(systemName: "magnifyingglass").foregroundStyle(.secondary)
                TextField("Find a tool…", text: $model.search).textFieldStyle(.plain).focused($searchFocused)
                    .onChange(of: model.search) { _, value in if !value.isEmpty { model.enterSearch() } }
                Text("⌘ K").font(.system(size: 10, weight: .medium)).foregroundStyle(.tertiary)
            }.font(.system(size: 12)).padding(.horizontal, 12).frame(width: 240, height: 32).atlasGlass(10)
            Button { model.library(); searchFocused = true } label: { EmptyView() }.buttonStyle(.plain).keyboardShortcut("k").frame(width: 0, height: 0).accessibilityHidden(true)
            Button { model.showQueue = true } label: {
                HStack(spacing: 5) { Image(systemName: "tray.2"); if model.busyCount > 0 { Text("\(model.busyCount)") } }
            }.buttonStyle(.plain).help("Job queue").accessibilityLabel("Open job queue")
        }.padding(.horizontal, 28).frame(height: 65)
    }

    private var library: some View {
        ScrollView {
            VStack(alignment: .leading, spacing: 25) {
                if model.section == "All tools" && model.search.isEmpty {
                    HStack(alignment: .center, spacing: 24) {
                        VStack(alignment: .leading, spacing: 13) {
                            Eyebrow(text: "A little less friction")
                            Text("Make room for\nbetter work.").font(.system(size: 42, weight: .bold)).tracking(-1.6).lineSpacing(-3)
                            Text("Useful tools. Thoughtfully together.").font(.system(size: 13)).foregroundStyle(.secondary)
                        }
                        Spacer()
                        VStack(spacing: 13) {
                            Image(systemName: "square.and.arrow.down.on.square").font(.system(size: 27, weight: .light)).foregroundStyle(AtlasStyle.accent)
                            Text(dropped.isEmpty ? "Drop files to get started" : "\(dropped.count) files ready").font(.system(size: 12, weight: .semibold))
                            Text("Or choose a tool below").font(.system(size: 10)).foregroundStyle(.secondary)
                        }.frame(width: 220, height: 135).atlasGlass(20)
                            .overlay(RoundedRectangle(cornerRadius: 20).strokeBorder(AtlasStyle.accent.opacity(0.3), style: StrokeStyle(lineWidth: 1, dash: [5])))
                            .dropDestination(for: URL.self) { urls, _ in dropped = urls; return !urls.isEmpty }
                    }.padding(30).frame(maxWidth: .infinity, alignment: .leading)
                        .background(LinearGradient(colors: [AtlasStyle.accent.opacity(0.13), .orange.opacity(0.045), .clear], startPoint: .topLeading, endPoint: .bottomTrailing), in: RoundedRectangle(cornerRadius: 24))
                    if !dropped.isEmpty {
                        VStack(alignment: .leading, spacing: 10) {
                            HStack { Eyebrow(text: "Suggested for your files"); Spacer(); Button("Clear") { dropped = [] }.buttonStyle(.plain) }
                            ScrollView(.horizontal, showsIndicators: false) {
                                HStack { ForEach(suggestedTools) { tool in
                                    Button { model.select(tool); model.addFiles(dropped); dropped = [] } label: { Label(tool.name, systemImage: tool.icon).padding(10).atlasPanel(10) }.buttonStyle(.plain)
                                } }
                            }
                        }
                    }
                }
                HStack {
                    Eyebrow(text: model.search.isEmpty ? "\(model.section) · \(model.visibleTools.count)" : "Search results · \(model.visibleTools.count)")
                    Spacer()
                    Text("One place. A lighter workflow.").font(.system(size: 10)).foregroundStyle(.tertiary)
                }
                if model.visibleTools.isEmpty {
                    ContentUnavailableView("No tools found", systemImage: "magnifyingglass", description: Text("Try a different search or add a favorite with the star button."))
                }
                LazyVGrid(columns: [GridItem(.adaptive(minimum: 240), spacing: 15)], spacing: 15) {
                    ForEach(model.visibleTools) { tool in toolCard(tool) }
                }
            }.padding(28)
        }
    }
    private var suggestedTools: [AtlasTool] {
        let extensions = Set(dropped.map { $0.pathExtension.lowercased() })
        let category = extensions == ["pdf"] ? "PDF" : extensions.isSubset(of: ["png", "jpg", "jpeg", "webp", "gif", "bmp", "tif", "tiff"]) ? "Images" : "Files"
        return toolCatalog.filter { $0.category == category && $0.acceptsFiles }
    }
    private func toolCard(_ tool: AtlasTool) -> some View {
        Button { model.select(tool) } label: {
            VStack(alignment: .leading, spacing: 12) {
                ToolBadge(tool: tool)
                Text(tool.name).font(.system(size: 15, weight: .semibold)).foregroundStyle(.primary)
                Text(tool.summary).font(.system(size: 11)).foregroundStyle(.secondary).lineLimit(2).frame(height: 31, alignment: .top)
                HStack { Text(tool.category.uppercased()).font(.system(size: 9, weight: .bold)).tracking(1); Spacer(); Image(systemName: "arrow.up.right") }.foregroundStyle(.tertiary)
            }.frame(maxWidth: .infinity, alignment: .leading).padding(19).atlasPanel(19).contentShape(Rectangle())
        }.buttonStyle(.plain)
        .overlay(alignment: .topTrailing) {
            Button { model.favorite(tool.id) } label: {
                Image(systemName: model.settings.favorites.contains(tool.id) ? "star.fill" : "star")
                    .foregroundStyle(model.settings.favorites.contains(tool.id) ? AtlasStyle.accent : .secondary)
                    .frame(width: 38, height: 38).contentShape(Rectangle())
            }.buttonStyle(.plain).padding(10).help("Favorite \(tool.name)").accessibilityLabel("Favorite \(tool.name)")
        }
    }
    private var recipes: some View {
        VStack(alignment: .leading, spacing: 18) {
            HStack { Text("Saved recipes").font(.title2.bold()); Spacer(); Button("Done") { showRecipes = false } }
            Text("Tool recipes save settings. Saved commands also keep the executable, arguments and working folder.").font(.callout).foregroundStyle(.secondary)
            List {
                ForEach(model.settings.recipes) { recipe in
                    HStack {
                        Button { model.loadRecipe(recipe); showRecipes = false } label: { VStack(alignment: .leading) { Text(recipe.name); Text(toolCatalog.first { $0.id == recipe.tool }?.name ?? "").font(.caption).foregroundStyle(.secondary) } }.buttonStyle(.plain)
                        Spacer()
                        Button { model.settings.recipes.removeAll { $0.id == recipe.id }; model.persist() } label: { Image(systemName: "trash") }.buttonStyle(.plain).help("Delete recipe")
                    }
                }
            }
            HStack { Button("Import settings") { model.importSettings() }; Button("Export settings") { model.exportSettings() }; Spacer() }
        }.padding(24)
    }
}

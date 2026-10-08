import SwiftUI
import UniformTypeIdentifiers

struct AtlasView: View {
    @ObservedObject var model: Workbench
    @FocusState private var searchFocused: Bool
    @State private var showRecipes = false
    @State private var showSidebar = false
    @Environment(\.accessibilityReduceMotion) private var reduceMotion
    @State private var dropped: [URL] = []

    var body: some View {
        GeometryReader { geometry in
            let compact = geometry.size.width < 1000
            HStack(spacing: 0) {
                if !compact { sidebar; Divider() }
                VStack(spacing: 0) {
                    toolbar(compact: compact)
                    Divider().opacity(0.5)
                    Group {
                        if let tool = model.selectedTool { ToolWorkspace(model: model, tool: tool).id(tool.id) }
                        else if model.section == "Settings" { AppearanceSettings(model: model) }
                        else { library }
                    }
                    .id(model.selectedID ?? model.section)
                    .transition(.opacity)
                }.frame(maxWidth: .infinity, maxHeight: .infinity).background(AtlasStyle.background)
            }
            .overlay(alignment: .leading) {
                if compact && showSidebar {
                    ZStack(alignment: .leading) {
                        Color.black.opacity(0.2).onTapGesture { showSidebar = false }
                        sidebar.shadow(radius: 12)
                    }.transition(reduceMotion ? .opacity : .move(edge: .leading).combined(with: .opacity))
                }
            }
            .animation(reduceMotion ? nil : .easeInOut(duration: 0.18), value: model.selectedID)
            .animation(reduceMotion ? nil : .easeInOut(duration: 0.18), value: model.section)
            .animation(reduceMotion ? nil : .easeInOut(duration: 0.18), value: showSidebar)
        }
        .tint(AtlasStyle.accent)
        .frame(minWidth: 640, minHeight: 440)
        .sheet(isPresented: $model.showQueue) { JobQueueView(model: model).frame(width: 580, height: 380) }
        .sheet(isPresented: $showRecipes) { recipes.frame(width: 530, height: 380) }
        .alert("Atlas", isPresented: Binding(get: { model.notice != nil }, set: { if !$0 { model.notice = nil } })) {
            Button("OK") { model.notice = nil }
        } message: { Text(model.notice ?? "") }
    }

    private var sidebar: some View {
        ScrollView {
        VStack(alignment: .leading, spacing: 6) {
            AtlasWordmark().padding(.horizontal, 12).padding(.top, 30).padding(.bottom, 23)
            nav("All tools", icon: "square.grid.2x2")
            nav("Favorites", icon: "star")
            nav("Recent", icon: "clock.arrow.circlepath")
            Eyebrow(text: "Collections").padding(.leading, 13).padding(.top, 25).padding(.bottom, 8)
            ForEach(["Files", "Images", "PDF", "Text", "Technical"], id: \.self) { category in nav(category, icon: AtlasStyle.icon(category)) }
            Divider().padding(.vertical, 12)
            comingSoon("Applications", icon: "shippingbox")
            comingSoon("Snippets", icon: "text.bubble")
            Spacer(minLength: 12)
            nav("Settings", icon: "gearshape")
            Button { showSidebar = false; showRecipes = true } label: {
                Label("Saved recipes", systemImage: "slider.horizontal.3").font(.system(size: 12, weight: .medium)).padding(12).frame(maxWidth: .infinity, alignment: .leading).contentShape(Rectangle())
            }.buttonStyle(.plain)
            HStack(spacing: 9) {
                Circle().fill(.green).frame(width: 5, height: 5)
                VStack(alignment: .leading, spacing: 3) {
                    Text("Your Mac. Your tools.").font(.system(size: 11, weight: .semibold))
                    Text("Atlas 1.0").font(.system(size: 10)).foregroundStyle(.secondary)
                }
            }.padding(14).frame(maxWidth: .infinity, alignment: .leading).atlasPanel().padding(.bottom, 18)
        }.padding(.horizontal, 14)
        }.frame(width: 202).frame(maxHeight: .infinity).background(AtlasStyle.background)
    }

    private func nav(_ title: String, icon: String) -> some View {
        let active = model.selectedID == nil && model.section == title
        let color = ["Files", "Images", "PDF", "Text", "Technical"].contains(title) ? AtlasStyle.color(title) : AtlasStyle.accent
        return Button { model.library(title); dropped = []; showSidebar = false } label: {
            HStack(spacing: 11) {
                Image(systemName: icon).font(.system(size: 14)).frame(width: 19)
                Text(title).font(.system(size: 12, weight: active ? .semibold : .medium))
                Spacer()
                if ["Files", "Images", "PDF", "Text", "Technical"].contains(title) {
                    Text("\(toolCatalog.filter { $0.category == title }.count)").font(.system(size: 10, design: .monospaced)).foregroundStyle(.secondary)
                }
            }.padding(.horizontal, 12).frame(height: 36).contentShape(Rectangle())
                .foregroundStyle(active ? color : Color.primary.opacity(0.78))
                .background(active ? color.opacity(0.10) : .clear, in: RoundedRectangle(cornerRadius: 10))
        }.buttonStyle(.plain)
    }

    private func comingSoon(_ title: String, icon: String) -> some View {
        Button {} label: {
            HStack(spacing: 10) {
                Image(systemName: icon).frame(width: 19)
                VStack(alignment: .leading, spacing: 2) {
                    Text(title).font(.system(size: 12))
                    Text("Coming soon").font(.system(size: 9))
                }
                Spacer()
            }.padding(.horizontal, 12).frame(height: 42).contentShape(Rectangle())
        }.buttonStyle(.plain).disabled(true).foregroundStyle(.secondary).opacity(0.65)
    }
    private func toolbar(compact: Bool) -> some View {
        HStack(spacing: 8) {
            if compact { IconButton("Show sidebar", icon: "sidebar.left") { showSidebar.toggle() } }
            if model.selectedTool != nil {
                IconButton("Back to category", icon: "chevron.left") { model.backToCategory() }
                Text(model.selectedTool?.category ?? "Workbench").font(.system(size: 12, weight: .medium)).foregroundStyle(.secondary)
            } else { Text(model.section).font(.system(size: 12, weight: .semibold)) }
            Spacer()
            HStack(spacing: 7) {
                Image(systemName: "magnifyingglass").foregroundStyle(.secondary)
                TextField("Find a tool…", text: $model.search).textFieldStyle(.plain).focused($searchFocused)
                    .onChange(of: model.search) { _, value in if !value.isEmpty { model.enterSearch() } }
                Text("⌘ K").font(.system(size: 10, weight: .medium)).foregroundStyle(.tertiary)
            }.font(.system(size: 12)).padding(.horizontal, 12).frame(minWidth: 110, maxWidth: 240).frame(height: 32).atlasGlass(10)
            Button { model.library(); searchFocused = true } label: { EmptyView() }.buttonStyle(.plain).keyboardShortcut("k").frame(width: 0, height: 0).accessibilityHidden(true)
            IconButton("Open job queue", icon: "tray.2") { model.showQueue = true }.overlay(alignment: .topTrailing) { if model.busyCount > 0 { Text("\(model.busyCount)").font(.caption2).foregroundStyle(AtlasStyle.accent) } }
        }.padding(.horizontal, 16).frame(height: 58)
    }

    private var library: some View {
        ScrollView {
            VStack(alignment: .leading, spacing: 25) {
                if model.section == "All tools" && model.search.isEmpty {
                    ViewThatFits(in: .horizontal) {
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
                    }
                    VStack(alignment: .leading, spacing: 10) {
                        Eyebrow(text: "A little less friction")
                        Text("Make room for better work.").font(.system(size: 30, weight: .bold))
                        Text("Useful tools. Thoughtfully together.").foregroundStyle(.secondary)
                    }
                    }.padding(24).frame(maxWidth: .infinity, alignment: .leading)
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
                        Button { model.loadRecipe(recipe); showRecipes = false } label: { VStack(alignment: .leading) { Text(recipe.name); Text(toolCatalog.first { $0.id == recipe.tool }?.name ?? "").font(.caption).foregroundStyle(.secondary) }.frame(maxWidth: .infinity, alignment: .leading).padding(.vertical, 8).contentShape(Rectangle()) }.buttonStyle(.plain)
                        Spacer()
                        Button { model.settings.recipes.removeAll { $0.id == recipe.id }; model.persist() } label: { Image(systemName: "trash") }.buttonStyle(.plain).help("Delete recipe")
                    }
                }
            }
            HStack { Button("Import settings") { model.importSettings() }; Button("Export settings") { model.exportSettings() }; Spacer() }
        }.padding(24)
    }
}

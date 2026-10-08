import SwiftUI
import AppKit
import ImageIO

struct ToolWorkspace: View {
    @ObservedObject var model: Workbench
    let tool: AtlasTool
    @State private var confirmRename = false
    @State private var confirmScript = false
    @State private var showRecipe = false
    @State private var showHTML = true

    var body: some View {
        VStack(alignment: .leading, spacing: 18) {
            HStack(alignment: .top, spacing: 14) {
                ToolBadge(tool: tool, size: 52)
                VStack(alignment: .leading, spacing: 5) {
                    Eyebrow(text: tool.category + " / Workbench")
                    Text(tool.name).font(.system(size: 27, weight: .bold)).tracking(-0.7)
                    Text(tool.summary).font(.system(size: 12)).foregroundStyle(.secondary).fixedSize(horizontal: false, vertical: true)
                }
                Spacer()
                Button { model.favorite(tool.id) } label: { Image(systemName: model.settings.favorites.contains(tool.id) ? "star.fill" : "star") }.buttonStyle(.plain).help("Favorite this tool")
                if !tool.options.isEmpty || tool.id == "scripts.run" {
                    Button { showRecipe = true } label: { Image(systemName: "slider.horizontal.3") }.buttonStyle(.plain).help("Save these settings as a recipe")
                }
            }
            HSplitView {
                ScrollView {
                    VStack(alignment: .leading, spacing: 19) {
                        if tool.acceptsFiles { fileInput }
                        if !tool.noText { textInput }
                        if !tool.options.isEmpty { options }
                        if tool.needsOutput {
                            VStack(alignment: .leading, spacing: 9) {
                                Eyebrow(text: tool.id == "files.rename" ? "Folder for undo record" : tool.id == "scripts.run" ? "Working folder" : "Output folder")
                                Button { model.chooseOutput() } label: {
                                    HStack { Image(systemName: "folder"); Text(model.request.outputDirectory.isEmpty ? "Choose folder…" : model.request.outputDirectory).lineLimit(2).truncationMode(.middle); Spacer(); Image(systemName: "chevron.down") }.font(.system(size: 11)).padding(12).atlasPanel(10).contentShape(Rectangle())
                                }.buttonStyle(.plain)
                            }
                        }
                        if tool.id == "text.workflow" { Text("Steps: trim, empty, unique, sort, number, upper, lower, replace. Steps run from left to right.").font(.caption).foregroundStyle(.secondary) }
                    }.padding(2)
                }.frame(minWidth: 285, idealWidth: 340, maxWidth: 480).padding(.trailing, 14)
                resultPanel.frame(minWidth: 300, maxWidth: .infinity, maxHeight: .infinity).padding(.leading, 14)
            }
            HStack(spacing: 12) {
                PrimaryButton(title: tool.id == "files.rename" ? "Preview names" : tool.id == "scripts.run" ? "Preview command" : "Run tool") { model.enqueue() }.keyboardShortcut(.return, modifiers: .command)
                if tool.id == "scripts.run" { Button("Execute command…") { confirmScript = true }.disabled(!model.canRunScript) }
                if tool.id == "files.rename" {
                    Button("Apply rename…") { confirmRename = true }.disabled(!model.canApplyRename)
                }
                if let job = model.currentJob, !job.isFinished {
                    ProgressView(value: job.progress).frame(width: 90)
                    Text(job.state).font(.caption).foregroundStyle(.secondary)
                    Button("Stop") { model.cancel(job.id) }
                }
                Spacer()
                Label(["tech.dns", "tech.tls", "tech.http", "scripts.run"].contains(tool.id) ? "Uses your network" : "Processed on your Mac", systemImage: "lock.shield")
                    .font(.system(size: 10)).foregroundStyle(.secondary)
            }
        }.padding(26)
        .confirmationDialog("Apply the previewed filenames?", isPresented: $confirmRename) {
            Button("Rename files") { model.enqueue(applyRename: true) }
        } message: { Text("Atlas will rename the selected files in place and save an undo record. Existing names are checked again before any change.") }
        .confirmationDialog("Execute the reviewed command?", isPresented: $confirmScript) {
            Button("Execute command") { model.enqueue(confirmedScript: true) }
        } message: { Text("The command runs with your Mac account's permissions in the chosen working folder. Review the executable and arguments first.") }
        .sheet(isPresented: $showRecipe) {
            VStack(alignment: .leading, spacing: 18) {
                Text("Keep this recipe").font(.title2.bold())
                Text(tool.id == "scripts.run" ? "Save this executable, its arguments and working folder. Avoid including passwords or tokens. Loading a command still requires preview and confirmation." : "Save the current steps and options. File paths and input contents are not saved.").font(.callout).foregroundStyle(.secondary)
                TextField("Recipe name", text: $model.recipeName).textFieldStyle(.roundedBorder)
                HStack { Button("Cancel") { showRecipe = false }; Spacer(); PrimaryButton(title: "Save recipe", icon: "bookmark") { model.saveRecipe(); if model.notice == nil { showRecipe = false } } }
            }.padding(25).frame(width: 400)
        }
    }

    private var fileInput: some View {
        VStack(alignment: .leading, spacing: 10) {
            HStack { Eyebrow(text: "Input files · \(model.request.paths.count)"); Spacer(); Button("Add…") { model.chooseFiles() }.buttonStyle(.plain).font(.caption) }
            VStack(spacing: 8) {
                if model.request.paths.isEmpty {
                    Image(systemName: "tray.and.arrow.down").font(.system(size: 26, weight: .light)).foregroundStyle(AtlasStyle.accent).padding(.top, 19)
                    Text("Drop files here").font(.system(size: 12, weight: .medium))
                    Button("Browse files") { model.chooseFiles() }.buttonStyle(.plain).font(.caption).foregroundStyle(.secondary).padding(.bottom, 19)
                } else {
                    ForEach(Array(model.request.paths.enumerated()), id: \.offset) { index, path in
                        HStack(spacing: 8) {
                            Image(systemName: "doc").foregroundStyle(.secondary)
                            Text(URL(fileURLWithPath: path).lastPathComponent).font(.system(size: 11)).lineLimit(1).help(path)
                            Spacer()
                            if index > 0 { Button { model.request.paths.swapAt(index, index - 1) } label: { Image(systemName: "arrow.up") }.buttonStyle(.plain).help("Move up") }
                            Button { model.request.paths.remove(at: index) } label: { Image(systemName: "xmark") }.buttonStyle(.plain).help("Remove from selection")
                        }.padding(8)
                    }
                    Button("Clear selection") { model.request.paths = [] }.font(.caption).buttonStyle(.plain).foregroundStyle(.secondary).padding(.bottom, 8)
                }
            }.frame(maxWidth: .infinity).atlasPanel(14)
                .dropDestination(for: URL.self) { urls, _ in model.addFiles(urls); return !urls.isEmpty }
        }
    }

    private var textInput: some View {
        VStack(alignment: .leading, spacing: 10) {
            HStack {
                Eyebrow(text: "Input")
                Spacer()
                Button("Open…") { model.importText() }.buttonStyle(.plain).font(.caption)
                Button("Paste") { model.request.input = NSPasteboard.general.string(forType: .string) ?? "" }.buttonStyle(.plain).font(.caption)
            }
            TextEditor(text: $model.request.input).font(.system(size: 12, design: .monospaced)).scrollContentBackground(.hidden)
                .frame(minHeight: 185).padding(10).atlasPanel(14).accessibilityLabel(tool.inputHint)
                .overlay(alignment: .topLeading) {
                    if model.request.input.isEmpty { Text(tool.inputHint).font(.system(size: 11, design: .monospaced)).foregroundStyle(.tertiary).padding(15).allowsHitTesting(false) }
                }
            Text("\(model.request.input.count) characters · \(model.request.input.split(whereSeparator: { $0.isWhitespace }).count) words").font(.system(size: 10)).foregroundStyle(.tertiary)
            if let hint = tool.secondaryHint {
                Eyebrow(text: tool.id == "tech.regex" ? "Pattern" : "Second input")
                TextEditor(text: $model.request.secondary).font(.system(size: 12, design: .monospaced)).scrollContentBackground(.hidden).frame(minHeight: 100).padding(10).atlasPanel(14).accessibilityLabel(hint)
                    .overlay(alignment: .topLeading) { if model.request.secondary.isEmpty { Text(hint).font(.system(size: 11, design: .monospaced)).foregroundStyle(.tertiary).padding(15).allowsHitTesting(false) } }
            }
        }
    }

    private var options: some View {
        VStack(alignment: .leading, spacing: 12) {
            Eyebrow(text: "Options")
            ForEach(tool.options) { option in
                if option.toggle {
                    Toggle(option.title, isOn: Binding(get: { model.request.options[option.id] == "true" }, set: { model.request.options[option.id] = $0 ? "true" : "false" })).font(.system(size: 11)).toggleStyle(.switch).controlSize(.small)
                } else if !option.choices.isEmpty {
                    HStack { Text(option.title).font(.system(size: 11)).foregroundStyle(.secondary); Spacer(); Picker(option.title, selection: binding(option.id)) { ForEach(option.choices, id: \.self) { Text($0).tag($0) } }.labelsHidden().frame(maxWidth: 165) }
                } else {
                    VStack(alignment: .leading, spacing: 5) { Text(option.title).font(.system(size: 10)).foregroundStyle(.secondary); TextField(option.title, text: binding(option.id)).textFieldStyle(.roundedBorder).font(.system(size: 12)) }
                }
            }
        }
    }
    private func binding(_ key: String) -> Binding<String> {
        Binding(get: { model.request.options[key] ?? "" }, set: { model.request.options[key] = $0 })
    }

    private var resultPanel: some View {
        VStack(alignment: .leading, spacing: 14) {
            HStack {
                Eyebrow(text: "Result")
                Spacer()
                if let job = model.currentJob {
                    if tool.id == "text.markdown" { Button(showHTML ? "Source" : "Preview") { showHTML.toggle() }.buttonStyle(.plain).font(.caption) }
                    Button { NSPasteboard.general.clearContents(); NSPasteboard.general.setString(exportText(job), forType: .string) } label: { Image(systemName: "doc.on.doc") }.buttonStyle(.plain).help("Copy result")
                    Button { model.saveText(exportText(job), extension: tool.id == "text.csv" ? "csv" : tool.id == "text.markdown" ? "html" : "txt") } label: { Image(systemName: "square.and.arrow.up") }.buttonStyle(.plain).help("Save result")
                }
            }
            if let job = model.currentJob {
                HStack(spacing: 7) {
                    Image(systemName: job.state == "Failed" ? "exclamationmark.circle" : job.state == "Completed" ? "checkmark.circle" : "clock")
                    Text(job.state).font(.system(size: 11, weight: .semibold))
                    Spacer()
                }.foregroundStyle(job.state == "Failed" ? .red : .secondary)
                if !job.isFinished { ProgressView(value: job.progress); Text(job.message).font(.caption).foregroundStyle(.secondary) }
                if tool.id == "text.markdown" && showHTML && job.state == "Completed" {
                    HTMLPreview(html: job.text).frame(maxWidth: .infinity, maxHeight: .infinity)
                } else {
                    ScrollView([.vertical, .horizontal]) {
                        VStack(alignment: .leading, spacing: 16) {
                            if !job.text.isEmpty { Text(job.text).font(.system(size: 12, design: .monospaced)).textSelection(.enabled).foregroundStyle(job.state == "Failed" ? Color.red : Color.primary).fixedSize(horizontal: false, vertical: true).frame(maxWidth: 600, alignment: .leading) }
                            if tool.id == "images.batch", let after = job.files.first,
                               let before = job.rows.first(where: { $0.count > 1 && $0[1] == after })?.first {
                                HStack(alignment: .top) { ImageThumbnail(path: before, title: "BEFORE"); ImageThumbnail(path: after, title: "AFTER") }
                            }
                            if !job.rows.isEmpty { ResultTable(rows: job.rows) }
                            ForEach(job.files, id: \.self) { path in
                                HStack {
                                    Image(systemName: "doc")
                                    VStack(alignment: .leading, spacing: 3) { Text(URL(fileURLWithPath: path).lastPathComponent).font(.system(size: 11, weight: .medium)); Text(path).font(.system(size: 9)).foregroundStyle(.secondary).lineLimit(1) }
                                    Spacer()
                                    Button("Reveal") { NSWorkspace.shared.activateFileViewerSelecting([URL(fileURLWithPath: path)]) }.font(.caption)
                                }.frame(maxWidth: 600)
                            }
                        }.frame(maxWidth: .infinity, alignment: .leading)
                    }.defaultScrollAnchor(.topLeading).frame(maxWidth: .infinity, maxHeight: .infinity)
                }
            } else {
                VStack(spacing: 12) {
                    Image(systemName: "rectangle.and.pencil.and.ellipsis").font(.system(size: 32, weight: .ultraLight)).foregroundStyle(AtlasStyle.accent.opacity(0.6))
                    Text("A fresh start.").font(.system(size: 18, weight: .semibold))
                    Text("Your results will appear here.").font(.system(size: 12)).foregroundStyle(.secondary)
                    Text("⌘ ↩  to run").font(.system(size: 10, design: .monospaced)).foregroundStyle(.tertiary).padding(.top, 6)
                }.frame(maxWidth: .infinity, maxHeight: .infinity)
            }
        }.padding(18).atlasPanel(18)
    }
    private func exportText(_ job: ToolJob) -> String {
        if tool.id == "text.csv" || job.rows.isEmpty { return job.text }
        return job.text + "\n\n" + job.rows.map { $0.joined(separator: "\t") }.joined(separator: "\n")
    }
}

struct ResultTable: View {
    let rows: [[String]]
    var body: some View {
        LazyVStack(alignment: .leading, spacing: 0) {
            ForEach(Array(rows.enumerated()), id: \.offset) { index, row in
                HStack(alignment: .top, spacing: 0) {
                    ForEach(Array(row.enumerated()), id: \.offset) { _, value in
                        Text(value).font(.system(size: 10, weight: index == 0 ? .semibold : .regular, design: .monospaced))
                            .textSelection(.enabled).frame(width: 170, alignment: .leading).padding(8)
                    }
                }.background(index == 0 ? AtlasStyle.accent.opacity(0.09) : index % 2 == 0 ? Color.primary.opacity(0.025) : .clear)
                Divider()
            }
        }
    }
}

struct ImageThumbnail: View {
    let path: String
    let title: String
    private var thumbnail: NSImage? {
        guard let source = CGImageSourceCreateWithURL(URL(fileURLWithPath: path) as CFURL, nil),
              let cg = CGImageSourceCreateThumbnailAtIndex(source, 0, [kCGImageSourceCreateThumbnailFromImageAlways: true, kCGImageSourceThumbnailMaxPixelSize: 600, kCGImageSourceCreateThumbnailWithTransform: true] as CFDictionary) else { return nil }
        return NSImage(cgImage: cg, size: .zero)
    }
    var body: some View {
        VStack(alignment: .leading, spacing: 8) {
            Eyebrow(text: title)
            if let image = thumbnail { Image(nsImage: image).resizable().scaledToFit().frame(width: 170, height: 135).background(.primary.opacity(0.04), in: RoundedRectangle(cornerRadius: 8)) }
        }
    }
}

struct JobQueueView: View {
    @ObservedObject var model: Workbench
    @Environment(\.dismiss) private var dismiss
    var body: some View {
        VStack(alignment: .leading, spacing: 16) {
            HStack { Text("Job queue").font(.title2.bold()); Spacer(); Button("Clear finished") { model.clearFinishedJobs() }.disabled(!model.jobs.contains { $0.isFinished }); Button("Done") { dismiss() } }
            Text("Jobs run one at a time. Inputs and results remain in this session only.").font(.callout).foregroundStyle(.secondary)
            if model.jobs.isEmpty { ContentUnavailableView("Nothing in the queue", systemImage: "tray", description: Text("Run a tool to see progress here.")) }
            List(model.jobs.reversed()) { job in
                VStack(alignment: .leading, spacing: 8) {
                    HStack { Text(toolCatalog.first { $0.id == job.request.tool }?.name ?? job.request.tool).fontWeight(.semibold); Spacer(); Text(job.state).font(.caption).foregroundStyle(job.state == "Failed" ? .red : .secondary); if !job.isFinished { Button("Stop") { model.cancel(job.id) } } }
                    Button("Open result") { model.openJob(job); dismiss() }
                    if !job.isFinished { ProgressView(value: job.progress) }
                    Text(job.isFinished ? job.text : job.message).font(.caption).foregroundStyle(.secondary).lineLimit(3)
                }.padding(.vertical, 6)
            }
        }.padding(24)
    }
}

# Third-party notices

Atlas's own source is MIT licensed. Dependencies keep their own copyright and license terms.

| Component | Version | License / source |
|---|---|---|
| .NET runtime | 10.x | [MIT and runtime notices](https://github.com/dotnet/runtime/blob/main/LICENSE.TXT) |
| PdfPig | 0.1.15 | [Apache 2.0](https://github.com/UglyToad/PdfPig/blob/master/LICENSE) |
| YamlDotNet | 18.1.0 | [MIT](https://github.com/aaubry/YamlDotNet/blob/master/LICENSE.txt) |
| Markdig | 1.0.0 | [BSD 2-Clause](https://github.com/xoofx/markdig/blob/master/license.txt) |
| ImageSharp | 4.1.2 | [Six Labors license terms](https://github.com/SixLabors/ImageSharp/blob/main/LICENSE); see included upstream license |

ImageSharp is used by the image tools and image-to-PDF conversion. Version 4 requires a valid build-time key for Release builds. The repository does not contain a key. Do not bypass validation or commit a key; provide an appropriate key privately for Release builds. See [upstream build-time licensing documentation](https://docs.sixlabors.com/articles/imagesharp/).

Docker CLI, Compose, Colima and Homebrew are optional, separately installed software; their binaries are not included in Atlas. Images imported through Atlas are third-party applications and retain their own licenses. Atlas does not relicense imported image contents.

Before publishing a self-contained application, collect the full notices for the exact runtime and transitive packages included in that artifact. The current local development bundle is not a public distribution release.

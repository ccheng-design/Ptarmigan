# Ptarmigan

**Ptarmigan** is a personal development repository — a curated collection of scripts, prototypes, and tools spanning multiple creative platforms. It serves as both a sandbox for experimental coding and a live home for ongoing tool and plugin development.

The repo brings together computational design utilities, automation workflows, and a Grasshopper plugin under active development, all documented in one place.

This repo contains:

- Custom scripts for Rhino, Grasshopper, Houdini, Unreal, and more
- Internal tools and automation workflows
- A Grasshopper plugin currently in development (with documentation)
- Archived builds, release notes, and setup files

---

## 🌐 Documentation

A live site for documentation and plugin info is hosted here:

📖 **[Ptarmigan Documentation](https://clarkchengdesign.gitbook.io/home/)**

---

## 📦 Grasshopper Plugin: Ptarmigan

A core part of this repo is the Grasshopper plugin **PTARMIGAN**, a set of custom components built to streamline layout, detail, and naming workflows in Rhino and Grasshopper.

### ✅ Installation

1. Download the latest `.gha` release from the `_BUILDS` folder.
2. Place it in your Grasshopper `Libraries` folder.
3. Restart Rhino & Grasshopper.

> If the components don't load, right-click the `.gha` file → **Properties** → **Unblock**, then restart.

### 🔧 Commands Included

**Layout & Detail**
- CopyViewFromDetail
- DetailDisplay
- DetailLock / DetailUnlock
- DetailScale / DetailScaleFactor
- LayoutNames
- ResetPageViews
- SelDetailsInDoc
- ZoomExtentInDetail

**Naming & Layers**
- AutoName
- MoveToNewLayer
- NamesToLayers

**Selection**
- SelSameArea
- SelSameLength

**Utilities**
- Random Colors
- Scale1DIncrement
- UnHideAll
- UP2 / UP3
- VolumeToDot
- Ptarmigan

More details and usage examples can be found in the 📖 **[Ptarmigan Documentation](https://clarkchengdesign.gitbook.io/home/)**.

---

## 🗂 Repository Structure

```plaintext
ptarmigan/
├── 01_RHINO/          # Rhino automation scripts
├── 02_GH/             # Grasshopper components and development
├── 03_HOUDINI/        # VEX snippets and tool experiments
├── 04_UNREAL/         # Unreal scripting and Python automation
├── OTHER/             # Misc tools and utilities
├── _ARCHIVE/          # Older versions, legacy work
├── _BUILDS/           # Grasshopper plugin builds & release notes
├── .github/           # GitHub workflows
├── .githtml/          # Documentation site assets
├── LICENSE.txt
└── README.md
```

---

## 📄 License

See [LICENSE.txt](LICENSE.txt) for details.

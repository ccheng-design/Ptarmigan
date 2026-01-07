# Ptarmigan

**Ptarmigan** is a personal development repository — a curated collection of scripts, prototypes, and processes spanning multiple platforms and tools. It acts as both a sandbox for creative coding and a live documentation site for ongoing tool and plugin development.

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

A key part of this repo is the Grasshopper plugin **PTARMIGAN**, which includes custom components to extend workflow efficiency.

### ✅ Installation Instructions

1. Download the latest `.gha` release from the `_BUILDS` folder.
2. Place it in your Grasshopper `Libraries` folder.
3. Restart Rhino & Grasshopper.

### 🔧 Commands Included

- `AutoName`
- `LayoutNames`
- `MoveToNewLayer`
- `NametoLayers`
- `RandomColors`
- `SelSameArea`
- `UnhideAll`
- `UP2`
- `UP3`
- `VolumetoDot`
- `BOM`
- `BlocktoLayer`

More details and usage examples can be found in the 📖 **[Ptarmigan Documentation](https://clarkchengdesign.gitbook.io/home/)**

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

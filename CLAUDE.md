# NorthStar SSA - Claude Code Instructions

## Project Overview
This is Meta's **NorthStar SSA** open-source VR project, used as a reference/source for extracting systems into Bernard's own VR games. This is NOT a project we build or run — we extract from it.

## Extraction Targets
- **Kilwa VR** (`D:\BERNARD\UNITY PROJECTS (GAMES)\Kilwa VR`) — Unity 6000.0.70f1 (LTS, URP 17.0.x)
- **Baraza VR** (`D:\BERNARD\UNITY PROJECTS (GAMES)\Baraza VR`) — Unity 6000.4.2f1 (Tech stream, URP 17.4.0 — NOT "URP 18", Unity 6 keeps URP at major version 17)

## Key Extraction Rules
1. **Map the full dependency tree BEFORE exporting** — NorthStar has deep coupling (prefabs -> materials -> shaders -> subgraphs -> HLSL)
2. **Export entire folders**, not individual files — piecemeal exports cause endless missing references
3. **Large texture folders (600MB+)** — instruct manual copy, don't use MCP export
4. **Always check for Meta URP fork customizations** that need inlining (e.g., `EnvBRDFApproxNonmetal`)
5. **URP version compatibility** — NorthStar uses custom URP 17.0.4 fork. Kilwa VR (LTS) is compatible. Baraza VR (URP 17.4.0) sky system is compatible (audited 2026-04-12), PBR/underwater shaders need Meta function inlining.
6. **Always save .unitypackage with the `.unitypackage` extension** — CoPlay export_package doesn't add it automatically
7. **Always save task lists and progress to memory files BEFORE telling user it's safe to restart**

## Extraction Progress

### Sky System
- **Kilwa VR**: DONE — skybox shaders, textures (5 TGA + starmap), 60 sky materials imported. EnvironmentSystem wired (SkyboxUpdater enabled, SunDiskRenderer added, ReflectionProbe created). Scene saved.
- **Baraza VR**: NEXT — same 3 packages ready to import. URP 17.4 audit passed (sky code uses only standard APIs). Need to create custom night sky material based on `Cloudy Pre Dawn_3` + stars.

### Ocean/Underwater System
- **Kilwa VR**: DONE (previous extraction)
- **Baraza VR**: Not needed (no ocean in Baraza)

## NorthStar Architecture Quick Reference
- **Custom URP fork**: `Packages/com.unity.render-pipelines.universal/` (v17.0.4)
- **Environment package**: `com.meta.utilities.environment` (git dependency)
- **Environment profiles**: `Assets/NorthStar/Data/Environment/`
- **Skybox shaders**: `Assets/NorthStar/Shaders/Skybox/` (SG_Skybox_waves.shadergraph + 2 subgraphs)
- **Sky materials**: `Assets/NorthStar/Data/Environment/Sky/` (60 materials + T_Starmap_02.tga)
- **Sky textures**: `Assets/NorthStar/Textures/Sky/` (5 cloud TGA files)
- **Ocean materials**: `Assets/NorthStar/Data/Environment/Ocean/`
- **Key scenes**: Launch, FreeSailing, Beat7 (underwater)

## Wiring Pattern (for target projects)
When setting up the sky system in a target project:
1. Import skybox shaders, textures, materials
2. Add `SunDiskRenderer` component to Environment GameObject
3. Wire `EnvironmentSystem.m_sunDiskRenderer` via SerializedObject (private field)
4. Enable `SkyboxUpdater.m_isEnabled` via SerializedObject (private field)
5. Create ReflectionProbe GameObject, wire to `SkyboxUpdater.m_reflectionProbe` (private field)
6. Assign sky material to EnvironmentProfile's `<SkyboxMaterial>k__BackingField`
7. Save scene

## MCP Connection
- When working on NorthStar: project root is this directory
- When working on Kilwa VR: call `set_unity_project_root` with `D:\BERNARD\UNITY PROJECTS (GAMES)\Kilwa VR`
- When working on Baraza VR: call `set_unity_project_root` with `D:\BERNARD\UNITY PROJECTS (GAMES)\Baraza VR`

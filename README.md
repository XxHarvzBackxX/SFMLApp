# SFMLApp

SFMLApp is my experimental 3D renderer, written in C# with SFML.NET. It started as a way to explore 3D graphics outside a traditional game engine and learn how the underlying maths and rendering steps fit together.

The current demo renders some bumpy terrain, a cube, a sphere, and several colourful animated lights. Everything is projected into 2D and drawn through SFML. It is still very much a hobby project, not a serious production-ready engine!

## Current features

- Perspective projection with a movable, mouse-controlled camera
- Cube, UV sphere, plane, and visible light-source primitives
- Model, world, view, and screen-space transformations
- Backface and basic camera-frustum culling
- Painter's-algorithm depth sorting
- Per-vertex diffuse lighting from multiple coloured point lights, including distance attenuation and ambient brightness
- Procedurally deformed grid terrain
- Delta-time-based orbits and vertical oscillation for scene objects
- Resizable, antialiased SFML window
- Optional face-edge and centroid debug overlay

## Controls

| Input | Action |
| --- | --- |
| `W` / `A` / `S` / `D` | Move the camera horizontally |
| `Q` / `E` | Move the camera up / down |
| Hold right mouse button and drag | Look around |
| Arrow keys | Move the cube on the X/Z axes |
| `V` / `B` | Move the cube on the Y axis |
| `R` / `F` | Roll the cube |
| `T` / `G` | Pitch the cube |
| `Y` / `H` | Yaw the cube |
| `Delete` | Toggle the debug overlay |

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A platform supported by [SFML.NET 2.6.1](https://www.nuget.org/packages/SFML.Net/2.6.1)

SFML.NET depends on native CSFML libraries. The NuGet package supplies the required binaries for common desktop runtimes; if startup fails with a missing native-library error, install the matching CSFML libraries for your operating system and architecture.

## Build and run

From the repository root:

```powershell
dotnet restore src/SFMLApp.slnx
dotnet build src/SFMLApp.slnx
dotnet run --project src/SFMLApp/SFMLApp.csproj
```

## A quick tour of the code

- `Infrastructure` contains the camera, scene, lighting, animation, and drawing flow.
- `Shapes/Base` contains the shared building blocks for 3D shapes and faces.
- `Shapes/Primitives` contains the cube, sphere, plane, and light sources.
- `Shapes/LandscapeFeatures` contains the terrain deformation code.
- `Utility` contains the maths, projection, and SFML drawing helpers.

## Project status

This is a learning exercise and an active work in progress. The renderer is deliberately makeshift: it sorts faces instead of using a depth buffer, and objects can disappear at the edge of the camera because proper clipping is not implemented yet.

Expect bugs, odd visual edge cases, unfinished ideas, and plenty of experimentation. Feel free to explore, learn from it, or point out the nonsense :)

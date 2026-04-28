# Computational Visuals & Audio-Reactive Systems

<p align="center">
  <img src="Recordings/Image%20Sequence_002_0000.jpg" alt="Boids" height="420">
  &nbsp;
  <img src="Recordings/Image%20Sequence_007_0000.jpg" alt="CCA" height="420">
  &nbsp;
  <img src="Recordings/Image%20Sequence_009_0000.jpg" alt="Edge of Chaos" height="420">
  &nbsp;
  <img src="Recordings/Image%20Sequence_006_0000.jpg" alt="Physarum" height="420">
</p>
<p align="center">
  <em>Boids · Cyclic Cellular Automata · Edge of Chaos · Physarum</em>
</p>

[![Unity](https://img.shields.io/badge/Unity-2021.3+-000000?style=flat-square&logo=unity&logoColor=white)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-8.0+-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Compute Shaders](https://img.shields.io/badge/Compute_Shaders-HLSL-5586A4?style=flat-square)](https://docs.unity3d.com/Manual/class-ComputeShader.html)
[![FFT](https://img.shields.io/badge/FFT-Audio_Analysis-FF6F61?style=flat-square)](https://en.wikipedia.org/wiki/Fast_Fourier_transform)
[![GPU](https://img.shields.io/badge/GPU-Instanced_Rendering-77B829?style=flat-square)](https://docs.unity3d.com/Manual/GPUInstancing.html)

GPU-accelerated particle simulations — flocking, cellular automata, and physarum — driven by real-time FFT audio analysis to produce emergent, organic visual behaviours on mobile hardware.

## Objective

Build a high-performance system that transforms real-time audio input into complex visual simulations. Three distinct algorithms (Reynolds' Boids, Cyclic Cellular Automata, Physarum slime mold) are implemented entirely on the GPU via Compute Shaders, with FFT frequency-band data driving simulation parameters. The result is a set of audio-reactive visual systems capable of rendering tens of thousands of agents at interactive frame rates.

## Simulations

### 1. Flocking (Boids)

Implementation of Craig Reynolds' steering behaviours. Position and velocity calculations are dispatched to the GPU via `BoidsCarrier.cs` and `FlockingBehaviour.cs`.

The movement vector for each agent is the weighted sum of four steering forces:

$$\vec{V}_{final} = \vec{V}_{separation} + \vec{V}_{alignment} + \vec{V}_{cohesion} + \vec{V}_{avoidance}$$

| Force | Behaviour |
|:---|:---|
| Separation | Repulsive force inversely proportional to distance² within local radius $r$ |
| Alignment | Steers toward the average heading of neighbours |
| Cohesion | Steers toward the centre of mass of neighbours |
| Avoidance | Raycast-based terrain detection; steering adjusted via surface normal $\hat{n}$ of the hit point |

Collision avoidance computes the adjusted steering vector as:

$$\vec{V}_{\text{steer}} = \left( \vec{P}_{\text{hit}} + \hat{n} - \vec{P}_{\text{agent}} \right)_{\text{normalized}}$$

### 2. Cyclic Cellular Automata (CCA)

A discrete grid simulation where a cell with state $S$ is consumed by a neighbour with state $S+1 \mod N_{states}$. Supports both Moore and Von Neumann neighbourhoods with configurable range and threshold parameters.

### 3. Edge of Chaos (EOCCCA)

Explores Langton's Lambda ($\lambda$) parameter to find phase transitions where complex structures emerge. The transition rule table is generated probabilistically and flattened for GPU dispatch:

$$Index = a \times N^2 + b \times N + c$$

### 4. Physarum Agent Trails

Based on Jeff Jones' algorithm for slime mold approximation. The simulation runs in three stages:

| Stage | Operation |
|:---|:---|
| **Sensory** | Agents probe the grid at angles $\theta$, $-\theta$, and $0$ |
| **Motor** | Agents rotate toward the highest chemical concentration (trail value) |
| **Diffusion & Decay** | Convolution kernel blurs the texture; values decay by factor $\delta < 1.0$ per frame |

## Audio Analysis Pipeline

The `AudioData.cs` module converts time-domain audio signals into frequency-domain data via FFT (Blackman-Harris window to minimise spectral leakage). A buffering system smooths amplitude spikes to prevent visual jitter:

$$A_{buf}(t) = A_{buf}(t-1) - \Delta_{decay}$$

Frequency bands are mapped to simulation parameters:

| Mapping | Target |
|:---|:---|
| Amplitude → Material Smoothness | `Mathf.Lerp(min, max, amplitudeBuffer)` |
| Frequency Bands → Geometry Scale | Per-band modulation of agent/cell size |

## Architecture & Optimisation

| Technique | Purpose |
|:---|:---|
| `ComputeBuffer` | Raw struct data transfer between C# and HLSL, bypassing `GameObject` overhead |
| Struct Alignment | C# structs padded to 16-byte HLSL alignment rules |
| `DrawMeshInstancedIndirect` | Geometry rendered directly from GPU buffer, eliminating per-instance draw calls |
| Audio-Reactive Materials | Smoothness, scale, and colour driven by FFT frequency band data |


## Getting Started

```bash
git clone https://github.com/maybebool/Audioresponsive-Shaders.git
```

1. Open the project in Unity 2021.3+ (URP or HDRP recommended for Compute Shader support).
2. Attach `AudioData` to a GameObject with an `AudioSource` and assign an audio clip.
3. For Flocking: add `BoidsCarrier` to the scene, assign the `BoidPrefab`, and link the Compute Shader.
4. For CCA / Physarum: attach `CCA` or `AgentCCA` respectively and assign a compatible output material.

**Prerequisites:** Unity 2021.3+, C# 8.0+, GPU with Compute Shader support.

## Tech Stack

[![Unity](https://img.shields.io/badge/Unity-000000?style=flat-square&logo=unity&logoColor=white)](https://unity.com/)
[![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![HLSL](https://img.shields.io/badge/HLSL-5586A4?style=flat-square)](https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl)

| Category | Technology |
|:---|:---|
| Engine | Unity 2021.3+ (URP/HDRP) |
| Language | C# 8.0+ |
| GPU Compute | HLSL Compute Shaders — flocking, CCA, physarum kernels |
| Audio | Unity `AudioSource` FFT with Blackman-Harris windowing |
| Rendering | `Graphics.DrawMeshInstancedIndirect`, ComputeBuffer struct pipeline |

## Limitations & Future Work

The current system demonstrates audio-reactive emergent simulations on GPU. Possible extensions include:

- 3D volumetric cellular automata (extending CCA from 2D grid to 3D voxel space)
- Multi-species physarum with inter-species trail interaction
- MIDI / OSC input as an alternative to microphone FFT for live performance control
- Reaction-diffusion systems (Gray-Scott, Belousov-Zhabotinsky) as additional simulation modes
- VR integration for immersive audio-visual experiences on Meta Quest
- Temporal persistence with trail history visualisation across simulation frames

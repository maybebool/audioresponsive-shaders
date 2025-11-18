## Audioresponsive Compute Shaders

In this project, I'm using compute shaders to simulate various algorithms like BIODS and CCA on an Android build made in Unity. 

Demo: https://youtube.com/shorts/nPgiHUXxqQQ?feature=share

### Audioresponive Compute Shader Algorithms

#### BOIDS



#### Voronoi 

<img src="https://github.com/maybebool/Audioresponsive-Shaders/blob/main/Recordings/Image%20Sequence_005_0000.jpg" alt="Voronoi" height="500">

--------------------------------------------------------------------------------------

For the following algorithms, I want to mention the creator Arsiliath and his compute shader course https://store.notochord.life/products/compute-shaders-with-unity that I took for education. 

#### CCA (cyclic cellular automaton)



EOC (Edge of Chaos)



Agents



# Computational Visuals & Audio-Reactive Systems

## Overview
This repository demonstrates a high-performance, audio-reactive visual simulation system built in Unity. The project focuses on implementing complex mathematical algorithms—specifically Flocking behaviors, Cellular Automata, and Physarum simulations—accelerated via GPU Compute Shaders to handle high-density particle counts.

The system transforms real-time audio input into visual data using FFT (Fast Fourier Transform) analysis, driving organic, emergent behaviors in real-time.

## Key Features
* **GPU-Accelerated Flocking:** Reynolds' Boids algorithm implementation using Compute Shaders for massive swarm simulation.
* **Cyclic Cellular Automata (CCA):** 2D grid-based state simulations supporting Moore neighborhoods.
* **Physarum Trails:** Agent-based slime mold simulations using sensory/motor stages and diffuse/decay kernels.
* **Audio Analysis Engine:** Real-time frequency band separation with logarithmic smoothing.
* **Indirect Instancing:** Rendering optimization using `Graphics.DrawMeshInstancedIndirect`.

---

## Mathematical Deep Dive

### 1. Flocking Simulation (Boids)\
<img src="https://github.com/maybebool/Audioresponsive-Shaders/blob/main/Recordings/Image%20Sequence_002_0000.jpg" alt="Boids" height="500">\
The flocking system implements Craig Reynolds' steering behaviors. To maintain high performance with large populations, position and velocity calculations are dispatched to the GPU via `BoidsCarrier.cs` and `FlockingBehaviour.cs`.

**The Core Forces**
The movement vector $V$ for each agent is calculated by summing weighted steering forces:

$$\vec{V}_{final} = \vec{V}_{separation} + \vec{V}_{alignment} + \vec{V}_{cohesion} + \vec{V}_{avoidance}$$

* **Separation:** Repulsive force inversely proportional to the distance squared between neighbors within a local radius $r$.
* **Alignment:** Steers the agent towards the average heading ($\vec{forward}$) of neighbors.
* **Cohesion:** Steers the agent towards the average position (center of mass) of neighbors.
* **Collision Avoidance:** Utilizes raycasting (`RaycastType`) to detect terrain. Upon detection, the steering vector is adjusted using the surface normal $\hat{n}$ of the hit point:
    $$ \vec{V}_{\text{steer}} = \text{normalize} \left( \vec{P}_{\text{hit}} + \hat{n} - \vec{P}_{\text{agent}} \right) $$

### 2. Audio Signal Processing (FFT)
The `AudioData.cs` module utilizes the Fast Fourier Transform (FFT) to convert time-domain audio signals into frequency-domain data.

* **Spectrum Analysis:** Uses a Blackman-Harris window to minimize spectral leakage.
* **Buffer Smoothing:** To prevent visual jitter, a buffering system smooths amplitude spikes. The buffer falls off linearly when the current amplitude is lower than the buffered amplitude:
    $$A_{buf}(t) = A_{buf}(t-1) - \Delta_{decay}$$

### 3. Discrete Math & Chaos Theory
The project implements multiple forms of discrete grid simulations.

**Cyclic Cellular Automata (`CCA.cs`)**\
<img src="https://github.com/maybebool/Audioresponsive-Shaders/blob/main/Recordings/Image%20Sequence_007_0000.jpg" alt="CCA" height="500">\
A cyclic system where a cell with state $S$ is consumed by a neighbor with state $S+1$ modulo $N_{states}$.
* **Algorithm:** Supports both Moore and Von Neumann neighborhoods with adjustable range and threshold parameters.

**Edge of Chaos (`EOCCCA.cs`)**\
<img src="https://github.com/maybebool/Audioresponsive-Shaders/blob/main/Recordings/Image%20Sequence_009_0000.jpg" alt="EOC" height="500">\
Explores Langton’s Lambda ($\lambda$) parameter. The system generates a transition table based on a probability $\lambda$ to find the phase transition where complex structures emerge.
* **Compute Indexing:** The 3D transition rule table is flattened for the GPU:
    $$Index = a \times N^2 + b \times N + c$$

**Physarum / Agent Trails (`AgentCCA.cs`)**\
<img src="https://github.com/maybebool/Audioresponsive-Shaders/blob/main/Recordings/Image%20Sequence_006_0000.jpg" alt="Agents" height="500">\
Based on Jeff Jones' algorithm for slime mold approximation.
1.  **Sensory Stage:** Agents probe the grid at angles $\theta$, $-\theta$, and $0$.
2.  **Motor Stage:** Agents rotate toward the highest chemical concentration (trail value).
3.  **Diffusion & Decay:** A convolution kernel blurs the texture, and values are multiplied by a decay factor $\delta < 1.0$ every frame.

---

## Architecture & Optimization

### Compute Buffers & Indirect Drawing
To bypass the CPU overhead of `GameObject` transforms, the system uses `ComputeBuffer` to pass raw struct data (`BoidConductValues`) between C# and HLSL.
* **Struct Alignment:** C# structs are padded to align with HLSL memory rules (16-byte alignment).
* **Indirect Instancing:** `NoiseGridInstance.cs` uses `DrawMeshInstancedIndirect` to render geometry directly from the GPU buffer, eliminating draw-call overhead.

### Audio-Reactive Material System
The `NoiseAudio.cs` and `BoidsCarrier.cs` scripts bridge the audio data and the rendering pipeline.
* **Smoothness Mapping:** Maps audio amplitude to material smoothness: `Mathf.Lerp(min, max, amplitudeBuffer)`.
* **Scale Modulation:** Modulates geometry scale based on specific frequency bands.

---

## Setup & Usage

1.  **Audio:** Attach `AudioData` to a GameObject with an `AudioSource` and assign a clip.
2.  **Flocking:** Add `BoidsCarrier` to the scene. Assign the `BoidPrefab` and link the Compute Shader.
3.  **Simulation:** For CCA or Physarum, attach `CCA` or `AgentCCA` respectively, ensuring a compatible Material is assigned for output.

---

### Dependencies
* Unity 2021.3+ (URP/HDRP recommended for Compute Shaders)
* C# 8.0+



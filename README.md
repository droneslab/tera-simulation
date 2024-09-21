# moog-simulation
# Excavator Simulator in Unity

This project simulates an excavator in Unity using ROS integration and custom controllers for joints and tracks. It includes a detailed setup for controlling the excavator's movement and performing various tasks.

## Table of Contents
1. [Requirements](#requirements)
2. [Installation](#installation)
3. [Setup](#setup)
   - [Clone Repository](#clone-repository)
   - [Install Dependencies](#install-dependencies)
   - [Unity Setup](#unity-setup)
4. [ROS Integration](#ros-integration)
5. [Running the Simulator](#running-the-simulator)
6. [Troubleshooting](#troubleshooting)

---

## Requirements
Ensure you have the following software installed:

- [Unity](https://unity.com/download)
- [ROS Humble](http://docs.ros.org/en/humble/Installation.html)
- [Git LFS](https://git-lfs.github.com/)
- [Algoryx AGX Dynamics for Unity](https://us.download.algoryx.se/AGXUnity/documentation/current/index.html)
  
## Installation

### 1. Clone Repository
First, clone the repository to your local machine. Ensure Git LFS is set up before cloning to manage large files.

```bash
git lfs install
git clone https://github.com/droneslab/moog-simulation.git
cd moog-simulation
```

### 2. Unity Setup

### Unity
1. Open the project in Unity:
   - Open Unity Hub and click on "Add."
   - Select the `Unity/Excavator` folder from the cloned repository.
   - In Unity, navigate to the `Unity/Excavator/Assets/` directory in the Project window locate the main scene named `Excavator_Sim.unity` for simulating the excavator.

2. Ensure that the AGX Dynamics for Unity plugin downloaded earlier is installed by going to the menu bar and selecting **Assets -> Import Package -> Custom Package**.

### 3. Setting Up the ROS Connector
1. In the Unity menu bar, go to **Window -> Package Manager**, click the **+** icon, and select **Add package from git URL...**. Enter the following URL:  
   [https://github.com/Unity-Technologies/ROS-TCP-Connector?path=/com.unity.robotics.ros-tcp-connector](https://github.com/Unity-Technologies/ROS-TCP-Connector?path=/com.unity.robotics.ros-tcp-connector)
2. After the package is installed, the **Robotics** option will appear in the Unity menu bar.


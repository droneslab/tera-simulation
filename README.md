# moog-simulation
# Excavator Simulator in Unity

This project simulates an excavator in Unity using ROS integration and custom controllers for joints and tracks. It includes a detailed setup for controlling the excavator's movement and performing various tasks.

## Table of Contents
1. [Requirements](#requirements)
2. [Installation](#installation)
   - [Clone Repository](#clone-repository)
   - [Unity Setup](#unity-setup)
   - [ROS Setup](#)
3. [Running the Simulator](#running-the-simulator)
4. [Troubleshooting](#troubleshooting)

---

## Requirements
Ensure you have the following software installed:

- [Unity](https://unity.com/download)
- [ROS Humble](http://docs.ros.org/en/humble/Installation.html)
- [Algoryx AGX Dynamics for Unity](https://us.download.algoryx.se/AGXUnity/documentation/current/index.html)

## Unity Login
- Email: excavator.hardware@gmail.com
- User: UB-Excavator-Team
- Password: Droneslab123
## Installation

### 1. Clone Repository
First, clone the repository to your local machine.

```bash
git clone https://github.com/droneslab/moog-simulation.git
cd moog-simulation
```

### 2. Unity Setup

#### Unity
1. Open the project in Unity:
   - Open Unity Hub and click on "Add."
   - Select the `Unity/Excavator` folder from the cloned repository.
   - In Unity, navigate to the `Unity/Excavator/Assets/Scenes` directory in the Project window locate the scenes for simulating different environments.

2. Ensure that the AGX Dynamics for Unity plugin downloaded earlier is installed by going to the menu bar and selecting **Assets -> Import Package -> Custom Package**.

> You should see `AGXUnity`,`Robotics` and `UnitySensors` on the ribbon

<!-- ### Import Assets
1. In the Unity menu bar, go to **Window -> Asset Manager**
2. Select the **Excavator** file you want to import
3. Hit the dropdown in the bottom right. Press **Import To**
4. Import it to `<Project_Folder>\Unity\Excavator\Assets\Prefab` -->

### Configuring ROS TCP Endpoint
1. Click on the **Robotics** option to configure ROS settings and ROS Messages.
   ![image](https://github.com/user-attachments/assets/038e8f9d-c628-41d0-bf91-d7e2ec2cbbc7)
2. If `/joy_deltacan` is not present then generate Custom ROSMessages inside Unity by specifying the path to `Deltcan` package folder from the cloned repository.

### AGX Setup
1. Activate AGX License using AGX License Manager by clicking `AGXUnity-> License -> License Manager`. Input your ID and Activation Code. This will create `agx.lfc` License file which can be used for other projects.
2. AGX Troubleshooting - **AGXUnity -> Utils -> Update Cleanup**

### ROS2 Setup

1. To establish communication between ROS2 and Unity, the **ROS-TCP-Endpoint** package is required. The necessary packages are provided in the `sim_ws/` directory of the repository. Build the source files using the following commands:

   ```bash
   cd sim_ws
   colcon build
   source install/setup.bash
   ```
2. After building the workspace, establish the connection between Unity and ROS2 by running the following command:
   If you are running default IP addresses and ports you can just run the run file
   ```bash
   ros2 run ros_tcp_endpoint default_server_endpoint
   ```
   If you need custom addresses/ports use
   ```bash
   ros2 run ros_tcp_endpoint default_server_endpoint --ros-args -p ROS_IP:=<your_IP_address> -p ROS_TCP_PORT:=<your_port>
   ```
   Replace `<your_IP_address>` and `<your_port>` with the values specified during the ROS configuration in Unity. This will start the ROS TCP server, enabling communication between Unity and ROS2.

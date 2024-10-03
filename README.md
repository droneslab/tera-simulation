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
  
## Installation

### 1. Clone Repository
First, clone the repository to your local machine. Ensure Git LFS is set up before cloning to manage large files.

```bash
git clone https://github.com/droneslab/moog-simulation.git
cd moog-simulation
```

### 2. Unity Setup

#### Unity
1. Open the project in Unity:
   - Open Unity Hub and click on "Add."
   - Select the `Unity/Excavator` folder from the cloned repository.
   - In Unity, navigate to the `Unity/Excavator/Assets/` directory in the Project window locate the main scene named `Excavator_Sim.unity` for simulating the excavator.

2. Ensure that the AGX Dynamics for Unity plugin downloaded earlier is installed by going to the menu bar and selecting **Assets -> Import Package -> Custom Package**.

#### Setting Up Unity Packages
1. In the Unity menu bar, go to **Window -> Package Manager**, click the **+** icon, and select **Add package from git URL...**. Enter the following URLs (One at a time):  
   - [https://github.com/Unity-Technologies/ROS-TCP-Connector?path=/com.unity.robotics.ros-tcp-connector](https://github.com/Unity-Technologies/ROS-TCP-Connector?path=/com.unity.robotics.ros-tcp-connector)
   - [https://github.com/Field-Robotics-Japan/UnitySensors.git?path=/Assets/UnitySensors#v2.0.4](https://github.com/Field-Robotics-Japan/UnitySensors.git?path=/Assets/UnitySensors#v2.0.4)
   - [https://github.com/Field-Robotics-Japan/UnitySensors.git?path=/Assets/UnitySensorsROS#v2.0.4](https://github.com/Field-Robotics-Japan/UnitySensors.git?path=/Assets/UnitySensorsROS#v2.0.4)
3. In the Unity menu bar, go to **Window -> Package Manager**, click the **+** icon, and select **Add package by name**. Enter the following name **com.unity.asset-manager-for-unity**. Select **Add**.

### Import Assets
1. In the Unity menu bar, go to **Window -> Asset Manager**
2. Select the **Excavator** file you want to import
3. Hit the dropdown in the bottom right. Press **Import To**
4. Import it to **<Project_Folder>\Unity\Excavator\Assets\Prefab**

### Configuring ROS TCP Endpoint
1. After the packages are installed, the **Robotics** option will appear in the Unity menu bar and Open it to configure ROS settings and ROS Messages.
   ![image](https://github.com/user-attachments/assets/038e8f9d-c628-41d0-bf91-d7e2ec2cbbc7)
2. Generate Custom ROSMessages inside Unity by specifying the path to Deltcan package folder from the cloned repository.

### ROS2 Setup

1. To establish communication between ROS2 and Unity, the **ROS-TCP-Endpoint** package is required. The necessary packages are provided in the `sim_ws/` directory of the repository. Build the source files using the following commands:

   ```bash
   cd sim_ws
   colcon build
   source install/setup.bash
   ```
2. After building the workspace, establish the connection between Unity and ROS2 by running the following command:
   If you are running default IP addresses and ports you can just run the launch file
   ```bash
   ros2 launch ros_tcp_endpoint params.py
   ```
   or if you need to specify an IP address or port
   ```bash
   ros2 run ros_tcp_endpoint default_server_endpoint --ros-args -p ROS_IP:=<your_IP_address> -p ROS_TCP_PORT:=<your_port>
   ```
   Replace `<your_IP_address>` and `<your_port>` with the values specified during the ROS configuration in Unity. This will start the ROS TCP server, enabling communication between Unity and ROS2.

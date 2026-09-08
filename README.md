# moog-simulation

Unity-based excavator simulation with ROS 2 integration. This repository contains the Unity project, ROS 2 TCP endpoint workspace, excavator configuration, and sensor publishing setup for the TERA excavation autonomy simulation environment.

## Requirements

- Unity 2022.3 LTS
- ROS 2 Humble
- Algoryx AGX Dynamics for Unity and a valid AGX license
- `colcon` and the standard ROS 2 build tools

No shared Unity login is required for normal use. The project should import from a fresh clone using the packages declared in `Unity/Excavator/Packages/manifest.json`.

Do not commit AGX license files, Unity account credentials, activation IDs/passwords, tokens, or other local secrets. AGX licenses are expected to be installed or activated per machine through the AGX Unity license tools.

## Repository Layout

- `Unity/Excavator/`: Unity project.
- `sim_ws/`: ROS 2 workspace containing `ROS-TCP-Endpoint`, `deltacan`, and helper nodes.
- `excavator_config.yaml`: Runtime excavator and sensor configuration consumed by the Unity scene.
- `excavator_config_template.yaml`: Larger sensor configuration example.
- `CAD/` and `Examples/`: Supporting assets and examples.

## Setup

Clone the repository:

```bash
git clone https://github.com/droneslab/moog-simulation.git
cd moog-simulation
```

Open the Unity project:

1. Open Unity Hub.
2. Add `Unity/Excavator`.
3. Let Unity restore packages from `Packages/manifest.json`.
4. Open `Assets/Scenes/YAML_Scene.unity`.
5. Confirm the AGX license is active from `AGXUnity -> License -> License Manager`.

Build the ROS 2 workspace:

```bash
cd sim_ws
source /opt/ros/humble/setup.bash
colcon build
source install/setup.bash
```

Start the ROS TCP endpoint before pressing Play in Unity:

```bash
ros2 run ros_tcp_endpoint default_server_endpoint
```

For custom Unity ROS settings:

```bash
ros2 run ros_tcp_endpoint default_server_endpoint --ros-args -p ROS_IP:=<your_ip_address> -p ROS_TCP_PORT:=<your_port>
```

## Running

1. Start the ROS TCP endpoint.
2. Press Play in `YAML_Scene.unity`.
3. Inspect ROS topics:

```bash
ros2 topic list
```

The default config spawns two excavators. `excavator1` includes IMU and camera sensors; both excavators publish excavator state topics such as odometry, ground truth, and effector pose.

## Camera Topics

Camera sensors are configured in `excavator_config.yaml`. For `excavator1`, the current camera base topic is `/camera`, so ROS 2 topics are namespaced under the excavator:

```text
/excavator1/camera/color/image/compressed
/excavator1/camera/color/camera_info
/excavator1/camera/depth/image/compressed
/excavator1/camera/depth/camera_info
/excavator1/camera/depth/points
```

View the RGB stream:

```bash
ros2 run rqt_image_view rqt_image_view
```

Select:

```text
/excavator1/camera/color/image/compressed
```

RViz `Camera` displays require both the image topic and matching `camera_info`. If RViz drops messages because the camera frame is missing, publish a temporary transform:

```bash
ros2 run tf2_ros static_transform_publisher 0 0 0 0 0 0 odom excavator1/RGB_CAMERA_frame
```

Then set RViz `Fixed Frame` to `odom`.

In Unity Play mode, spawned sensors are named with their excavator prefix, for example `excavator1_RGB_CAMERA`. Enable Scene view `Gizmos` and select the object to see the camera marker and frustum.

## Sensor Configuration

Sensors are spawned by `Unity/Excavator/Assets/Scripts/Excavator_Creator.cs` from `excavator_config.yaml`. Each excavator entry can define a `sensors` list with:

- `id`: sensor instance name.
- `type`: one of the supported prefab types, such as `IMU`, `GPS`, `LIDAR`, `RGB_CAMERA`, or `RGBD_CAMERA`.
- `topic`: base topic. The spawner prefixes it with the excavator id.
- `location`: attachment target such as `CHASIS`, `BOOM`, `ARM`, or `BUCKET`.
- `offset` and `rotation`: local pose relative to the attachment target.

After changing `excavator_config.yaml`, restart Unity Play mode so the scene respawns sensors.

## Citation

If you use this simulator in academic work, cite:

```bibtex
@INPROCEEDINGS{10979147,
  author={Aluckal, Christo and Kumar Lal, Roopesh Vinodh and Courtney, Sean and Turkar, Yash and Dighe, Yashom and Kim, Youngjin and Gemerek, Jake and Dantu, Karthik},
  booktitle={2025 IEEE International Conference on Simulation, Modeling, and Programming for Autonomous Robots (SIMPAR)},
  title={TERA: A Simulation Environment for Terrain Excavation Robot Autonomy},
  year={2025},
  volume={},
  number={},
  pages={1-6},
  keywords={Deformation;Scalability;Programming;Excavation;Real-time systems;Extensibility;Complexity theory;Time-varying systems;Autonomous robots;Excavation;Simulation;Autonomy},
  doi={10.1109/SIMPAR62925.2025.10979147}
}
```

## Troubleshooting

- If Unity cannot connect to ROS 2, confirm `ros_tcp_endpoint` is running and Unity Robotics ROS settings match the endpoint IP and port.
- If camera topics appear in ROS 2 but RViz shows a blank/off-white view, first verify the image in `rqt_image_view`; then check RViz `CameraInfo` and TF frame settings.
- If Unity does not show AGX, Robotics, or UnitySensors menus after import, let package import finish and re-open the project.
- If AGX reports a missing license, activate or import a license locally through `AGXUnity -> License -> License Manager`; do not add license files to the repository.

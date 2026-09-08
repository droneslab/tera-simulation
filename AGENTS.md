# Agent Notes

This repository is a Unity + ROS 2 simulation project. Keep changes scoped and avoid editing generated Unity directories unless explicitly requested.

Project page: https://droneslab.github.io/tera/

## Project Shape

- Unity project: `Unity/Excavator/`
- ROS 2 workspace: `sim_ws/`
- Runtime excavator/sensor config: `excavator_config.yaml`
- Main YAML scene: `Unity/Excavator/Assets/Scenes/YAML_Scene.unity`
- Non-YAML deformation analysis scene: `Unity/Excavator/Assets/Scenes/SampleScene.unity`

## Important Rules

- Do not edit `Unity/Excavator/Library/`, `Temp/`, `Logs/`, or other generated Unity state.
- Do not modify package cache files under `Unity/Excavator/Library/PackageCache/`; package behavior should be adapted from repo-owned scripts.
- Preserve unrelated Unity-generated `.csproj`, `Temp`, and `UserSettings` changes unless the user explicitly asks to clean them.
- Commit Unity `.meta` files for new assets/scripts when Unity generates them.
- Never commit AGX license files, activation IDs/passwords, Unity account credentials, tokens, `.env` files, private keys, or other local secrets.
- Deformation behavior and deformable AGX entities require a valid AGX license. If deformation is failing, verify licensing before changing simulation logic.

## ROS 2 Workflow

Build and source the workspace:

```bash
cd sim_ws
source /opt/ros/humble/setup.bash
colcon build
source install/setup.bash
```

Run the Unity ROS TCP endpoint:

```bash
ros2 run ros_tcp_endpoint default_server_endpoint
```

Inspect topics:

```bash
ros2 topic list
ros2 topic info <topic> -v
```

## Camera Notes

Camera and sensor instances are spawned from `excavator_config.yaml` by `Excavator_Creator.cs`.

Expected `excavator1` camera topics:

```text
/excavator1/camera/color/image/compressed
/excavator1/camera/color/camera_info
/excavator1/camera/depth/image/compressed
/excavator1/camera/depth/camera_info
/excavator1/camera/depth/points
```

Use `rqt_image_view` to confirm the raw stream before debugging RViz. RViz `Camera` displays need matching `camera_info` and TF for the image header frame, currently `excavator1/RGB_CAMERA_frame`.

## Documentation

Keep the root `README.md` as the source of truth for setup and user-facing workflows. The old shared Unity login flow is obsolete and should not be reintroduced.

Document `YAML_Scene.unity` as the sensor-testing scene and `SampleScene.unity` as the non-YAML deformation-analysis scene unless the project structure changes.

#!/usr/bin/env python3

import rclpy
from rclpy.node import Node
from std_msgs.msg import Header
from deltacan.msg import DeltaCan

class ExcavatorCommandPublisher(Node):
    def __init__(self):
        super().__init__('deltacan_node')
        self.publisher_ = self.create_publisher(DeltaCan, 'excavator_command_topic', 10)
        timer_period = 1/30
        self.timer = self.create_timer(timer_period, self.publish_command)

    def publish_command(self):
        msg = DeltaCan()
        msg.header = Header()
        msg.header.stamp = self.get_clock().now().to_msg()
        msg.header.frame_id = "base_link"

        
        msg.mslewcmd = 0.0
        msg.mboomcmd = 0.0
        msg.mbucketcmd = 1.0
        msg.marmcmd = 0.0
        msg.mlefttravelcmd = 3.0
        msg.mrighttravelcmd = 3.0
        msg.mbladecmd_a = 0.0
        msg.mswingcmd_a = 0.0
        msg.mthumbcmd_a = 0.0
        msg.mbladecmd_b = 0.0
        msg.mswingcmd_b = 0.0
        msg.mthumbcmd_b = 0.0
        msg.mhydraulic = 0.0
        msg.mhorn = 0.0
        msg.menableautonav = 1.0

        self.publisher_.publish(msg)


def main(args=None):
    rclpy.init(args=args)
    excavator_publisher = ExcavatorCommandPublisher()

    try:
        rclpy.spin(excavator_publisher)
    except KeyboardInterrupt:
        pass
    finally:
        excavator_publisher.destroy_node()
        rclpy.shutdown()


if __name__ == '__main__':
    main()


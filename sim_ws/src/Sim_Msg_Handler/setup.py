from setuptools import find_packages, setup

package_name = 'Sim_Msg_Handler'

setup(
    name=package_name,
    version='0.0.0',
    packages=find_packages(exclude=['test']),
    data_files=[
        ('share/ament_index/resource_index/packages',
            ['resource/' + package_name]),
        ('share/' + package_name, ['package.xml']),
    ],
    install_requires=['setuptools'],
    zip_safe=True,
    maintainer='roop',
    maintainer_email='roopesh0831@gmail.com',
    description='ROS 2 helper nodes for the Unity excavator simulation.',
    license='Apache-2.0',
    tests_require=['pytest'],
    entry_points={
        'console_scripts': [
            'deltacan_node = Sim_Msg_Handler.deltacan_node:main'
        ],
    },
)

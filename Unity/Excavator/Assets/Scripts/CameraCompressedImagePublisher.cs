using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class CameraCompressedImagePublisher : MonoBehaviour
{
    public string topicName = "/camera/color/image/compressed";
    public string frameId = "camera_frame";
    public float publishFrequency = 10.0f;
    [Range(1, 100)]
    public int jpegQuality = 75;

    private Camera cameraComponent;
    private ROSConnection ros;
    private RenderTexture renderTexture;
    private Texture2D texture;
    private float timeElapsed;

    void Start()
    {
        cameraComponent = GetComponent<Camera>();
        if (cameraComponent == null)
        {
            Debug.LogError($"CameraCompressedImagePublisher on '{name}' requires a Camera component");
            enabled = false;
            return;
        }

        int width = Mathf.Max(1, cameraComponent.pixelWidth);
        int height = Mathf.Max(1, cameraComponent.pixelHeight);
        if (width <= 1 || height <= 1)
        {
            width = 640;
            height = 480;
        }

        renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        texture = new Texture2D(width, height, TextureFormat.RGB24, false);

        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<CompressedImageMsg>(topicName);
        Debug.Log($"Registering camera image publisher for topic: {topicName}");
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed < 1.0f / publishFrequency)
        {
            return;
        }

        PublishImage();
        timeElapsed = 0.0f;
    }

    void PublishImage()
    {
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture previousTarget = cameraComponent.targetTexture;

        cameraComponent.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;
        cameraComponent.Render();

        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        cameraComponent.targetTexture = previousTarget;
        RenderTexture.active = previousActive;

        CompressedImageMsg msg = new CompressedImageMsg
        {
            header = new HeaderMsg
            {
                stamp = new TimeMsg
                {
                    sec = (int)Time.time,
                    nanosec = (uint)((Time.time - (int)Time.time) * 1e9)
                },
                frame_id = frameId
            },
            format = "jpeg",
            data = texture.EncodeToJPG(jpegQuality)
        };

        ros.Publish(topicName, msg);
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }
}

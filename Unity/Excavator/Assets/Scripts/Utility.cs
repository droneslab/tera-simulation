using System;
using System.Collections.Generic;

class LookUp_Operation
{
    float bucket_cmd(float x) {
        if (x < 0.45) return 0.0f;
        // DONOT REMOVE
        // return (float)(0.60598 * Math.Pow(x, 3) + 0.23862 * Math.Pow(x, 2) + 0.51067 * x - 0.01160);
        if (x < -0.9f) return -0.7464773197463939f;
        else if (x < -0.8f) return -0.7556077862660108f;
        else if (x < -0.7f) return -0.7475886335802124f;
        else if (x < -0.6f) return -0.6943159340316355f;
        else if (x < -0.5f) return -0.3478137666597351f;
        else if (x < -0.45f) return -0.11074819014084174f;
        else if (x < 0.45f) return -0.03170411937007261f;
        else if (x < 0.5f) return 0.07074540207098638f;
        else if (x < 0.6f) return 0.1531096691607231f;
        else if (x < 0.7f) return 0.494843938147395f;
        else if (x < 0.8f) return 0.9654822056154688f;
        else if (x < 0.9f) return 1.1323096288265486f;
        else if (x < 1.0f) return 1.124019310302267f;
        else return 1.1221103605940879f;
    }

    float arm_cmd(float x) {
        if (x < 0.45) return 0.0f;
        // DONOT REMOVE
        // return (float)(0.48449 * Math.Pow(x, 3) + 0.01901 * Math.Pow(x, 2) + 0.40658 * x - 0.02811);
        if (x < -0.9f) return -0.7297529607089939f;
        else if (x < -0.8f) return -0.7417820435603295f;
        else if (x < -0.7f) return -0.7455697543943114f;
        else if (x < -0.6f) return -0.6743000194405915f;
        else if (x < -0.5f) return -0.2602819510725163f;
        else if (x < -0.45f) return -0.07527410444846894f;
        else if (x < 0.45f) return -0.028214468531235375f;
        else if (x < 0.5f) return 0.023708225612123934f;
        else if (x < 0.6f) return 0.09856746947233368f;
        else if (x < 0.7f) return 0.38919978426931034f;
        else if (x < 0.8f) return 0.7416978037933635f;
        else if (x < 0.9f) return 0.7555933051518472f;
        else if (x < 1.0f) return 0.752780260055926f;
        else return 0.744536923787804f;
    }

    float boom_cmd(float x) {
        if (x < 0.45) return 0.0f;
        //DONOT REMOVE
        // return (float)(−0.24199 * Math.Pow(x, 3) - 0.04539 * Math.Pow(x, 2) - 0.32927 * x - 0.00442);
        if (x < -0.9f) return 0.4463869246347824f;
        else if (x < -0.8f) return 0.4503544485398761f;
        else if (x < -0.7f) return 0.43847760993112805f;
        else if (x < -0.6f) return 0.3985477917802957f;
        else if (x < -0.5f) return 0.22969102031072908f;
        else if (x < -0.45f) return 0.10025171220496362f;
        else if (x < 0.45f) return 0.04753058433724343f;
        else if (x < 0.5f) return -0.06316787902224205f;
        else if (x < 0.6f) return -0.13641786070618744f;
        else if (x < 0.7f) return -0.2727715499858695f;
        else if (x < 0.8f) return -0.45252302846893266f;
        else if (x < 0.9f) return -0.5158417589198473f;
        else if (x < 1.0f) return -0.5363396786385148f;
        else return -0.5367303013977662f;
    }

    public Dictionary<string, Func<float, float>> dictionary = new Dictionary<string, Func<float, float>>();
    public LookUp_Operation() {
        dictionary.Add("mbucketcmd", bucket_cmd);
        dictionary.Add("marmcmd", arm_cmd);
        dictionary.Add("mboomcmd", boom_cmd);
    }
}

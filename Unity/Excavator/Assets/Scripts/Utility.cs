using System;
using System.Collections.Generic;

class LookUp_Operation
{
    float bucket_cmd(float x) {
        return (float)(0.61183 * Math.Pow(x, 3) + 0.24844 * Math.Pow(x, 2) + 0.5182 * x - 0.01318);
    }

    float arm_cmd(float x) {
        return (float)(0.47968 * Math.Pow(x, 3) + 0.01143 * Math.Pow(x, 2) + 0.41132 * x - 0.02371);
    }

    float boom_cmd(float x) {
        return (float)(0.99793 * Math.Pow(x, 3) + 0.13337 * Math.Pow(x, 2) + 1.41819 * x - 0.04810);
    }

    public Dictionary<string, Func<float, float>> dictionary = new Dictionary<string, Func<float, float>>();
    public LookUp_Operation() {
        dictionary.Add("mbucketcmd", bucket_cmd);
        dictionary.Add("marmcmd", arm_cmd);
        dictionary.Add("mboomcmd", boom_cmd);
    }
}

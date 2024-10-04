using System;
using System.Collections.Generic;

class LookUp_Operation
{
    float bucket_cmd(float x) {
        return (float)(0.6118 * Math.Pow(x, 3) + 0.2484 * Math.Pow(x, 2) + 0.5182 * x - 0.01318);
    }

    public Dictionary<string, Func<float, float>> dictionary = new Dictionary<string, Func<float, float>>();
    public LookUp_Operation() {
        dictionary.Add("mbucketcmd", bucket_cmd);
    }
}

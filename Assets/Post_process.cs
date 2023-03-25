using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Post_process : MonoBehaviour
{

    public ComputeShader textureShader;


    public float density = 1.0f;
   
    public float weight = 1.0f;
  
  
    public float decay = 1.0f;
   

    public float exposure = 1.0f;
   
    public int num_samples = 100;

    private RenderTexture _rTexture;
    public Light sun;
    Camera cam;
    private void OnRenderImage(RenderTexture source, RenderTexture destintion)
    {

        Vector3 sunScreenPos = cam.WorldToScreenPoint(sun.transform.position);
        float[] sunScreenArray = new float[3];
        for (int i = 0; i < 3; i++)
        {
            sunScreenArray[i] = sunScreenPos[i];
        }

        RenderTexture r = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        r.enableRandomWrite = true;
        r.Create();
        Graphics.Blit(source, r);


        _rTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        _rTexture.enableRandomWrite = true;
        _rTexture.Create();



        int kernel = textureShader.FindKernel("CSMain");

        //textureShader.SetFloats("sunScreenPos", sunScreenArray);
        //textureShader.SetFloat("density", density);
        //textureShader.SetFloat("weight", weight);
        //textureShader.SetFloat("decay", decay);
        //textureShader.SetFloat("exposure", exposure);
        //textureShader.SetInt("num_samples", num_samples);

        textureShader.SetTexture(kernel, "result_buffer", _rTexture);
        textureShader.SetTexture(kernel, "screen_buffer", r);

        int workgroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int workgroupsY = Mathf.CeilToInt(Screen.height / 8.0f);

        textureShader.Dispatch(kernel, workgroupsX, workgroupsY, 1);
        Graphics.Blit(_rTexture, destintion);

        r.Release();
        _rTexture.Release();
    }

    void Start()
    {
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        UpdateSettings();
    }

    void UpdateSettings()
    {
        density = Mathf.Clamp(density, 0.0f, 1.0f);
        weight = Mathf.Clamp(weight, 0.0f, 1.0f);
        decay = Mathf.Clamp(decay, 0.0f, 1.0f);
        exposure = Mathf.Max(exposure, 0.0f);
        num_samples = Mathf.Clamp(num_samples, 1, 500);

        // Set the shader variables here
        textureShader.SetFloat("density", density);
        textureShader.SetFloat("weight", weight);
        textureShader.SetFloat("decay", decay);
        textureShader.SetFloat("exposure", exposure);
        textureShader.SetInt("num_samples", num_samples);
    }

}
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class Render2DLights : MonoBehaviour 
{
	public Material renderMaterial = null;
	public Shader blurEffectShader;
	public Shader additiveShader;
	public Shader multiplicativeShader;
	public LayerMask playerLightLayer;
	public LayerMask pursuerLightLayer;
	public LayerMask fieldOfViewLayer;
	public int iterations = 3;
    public float blurSpread = 0.6f;
    public bool useSceneAmbientColor = false;
    public Color ambientColor = new Color(0.05f, 0.05f, 0.05f, 1);
	
	private RenderTexture _playerLightTexture;
	private RenderTexture _pursuerLightTexture;
	private RenderTexture _fieldOfViewTexture;
    private GameObject _lightCam;

    void CleanTexture()
    {
        if (_playerLightTexture) {
            RenderTexture.ReleaseTemporary(_playerLightTexture);
            _playerLightTexture = null;
		}
		if (_pursuerLightTexture) {
			RenderTexture.ReleaseTemporary(_pursuerLightTexture);
			_pursuerLightTexture = null;
		}
		if (_fieldOfViewTexture) {
			RenderTexture.ReleaseTemporary(_fieldOfViewTexture);
			_fieldOfViewTexture = null;
		}
    }

    protected void OnPreRender()
    {
        if (!renderMaterial || !enabled || !gameObject.activeSelf)
            return;
		
		CleanTexture ();
		_playerLightTexture = RenderTexture.GetTemporary((int)camera.pixelWidth, (int)camera.pixelHeight, 0, RenderTextureFormat.ARGB32);
		_pursuerLightTexture = RenderTexture.GetTemporary((int)camera.pixelWidth, (int)camera.pixelHeight, 0, RenderTextureFormat.ARGB32);
		_fieldOfViewTexture = RenderTexture.GetTemporary((int)camera.pixelWidth, (int)camera.pixelHeight, 0, RenderTextureFormat.ARGB32);

        if (!_lightCam) {
            _lightCam = new GameObject("LightCam", typeof(Camera));
            _lightCam.camera.enabled = false;
            _lightCam.hideFlags = HideFlags.HideAndDontSave;
        }

        Camera cam = _lightCam.camera;
        cam.CopyFrom(camera);
        cam.backgroundColor = useSceneAmbientColor ? RenderSettings.ambientLight : ambientColor;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.cullingMask = playerLightLayer;
        cam.targetTexture = _playerLightTexture;
        cam.Render();

		cam.cullingMask = pursuerLightLayer;
		cam.targetTexture = _pursuerLightTexture;
		cam.Render();

		cam.cullingMask = fieldOfViewLayer;
		cam.targetTexture = _fieldOfViewTexture;
		cam.Render();
    }
    
    void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!renderMaterial)
		{
			Debug.LogError("Render Material in Render2DLights must have a material assigned to it!");
			return;
		}
		
		if (!additiveShader)
		{
			Debug.LogError("additiveShader in Render2DLights must have a shader assigned to it!");
			return;
		}
		
		if (!additiveShader)
		{
			Debug.LogError("multiplicativeShader in Render2DLights must have a shader assigned to it!");
			return;
		}

		Material additiveMaterial = new Material (additiveShader);
		Material multiplicativeMaterial = new Material (multiplicativeShader);

		RenderTexture temp = RenderTexture.GetTemporary((int)camera.pixelWidth, (int)camera.pixelHeight, 0, RenderTextureFormat.ARGB32);
		Graphics.Blit (source, temp);


        if (blurEffectShader)
		{
			BlitBlurEffect(_playerLightTexture, source, renderMaterial);
			BlitBlurEffect(_pursuerLightTexture, temp, renderMaterial);
			BlitBlurEffect(_fieldOfViewTexture, temp, multiplicativeMaterial);
		}
        else
		{
			Graphics.Blit(_playerLightTexture, source, renderMaterial);
			Graphics.Blit(_pursuerLightTexture, temp, renderMaterial);
			Graphics.Blit(_fieldOfViewTexture, temp, multiplicativeMaterial);
		}
		Graphics.Blit (source, temp, additiveMaterial);
		Graphics.Blit(temp, destination);
        CleanTexture();

		RenderTexture.ReleaseTemporary (temp);
		DestroyImmediate (additiveMaterial);
		DestroyImmediate (multiplicativeMaterial);
    }

    void OnDisable()
    {
        CleanTexture();
        if (_lightCam)
        {
            DestroyImmediate(_lightCam);
        }
    }

    public void BlitBlurEffect(RenderTexture source, RenderTexture destination, Material material)
    {
        int rtW = source.width/4;
		int rtH = source.height/4;

		RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
		Material mat = new Material(blurEffectShader);

		// Copy source to the 4x4 smaller texture.
		DownSample4x(source, buffer, mat);
		
		// Blur the small texture
		for(int i = 0; i < iterations; i++)
		{
			RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0);
			FourTapCone (buffer, buffer2, mat, i);
			RenderTexture.ReleaseTemporary(buffer);
			buffer = buffer2;
		}
        Graphics.Blit(buffer, destination, material);

        RenderTexture.ReleaseTemporary(buffer);
        DestroyImmediate(mat);
    }

    // Performs one blur iteration.
    public void FourTapCone(RenderTexture source, RenderTexture dest, Material material, int iteration)
    {
        float off = 0.5f + iteration * blurSpread;
        Graphics.BlitMultiTap(source, dest, material,
            new Vector2(-off, -off),
            new Vector2(-off, off),
            new Vector2(off, off),
            new Vector2(off, -off)
        );
    }

    // Downsamples the texture to a quarter resolution.
    private void DownSample4x(RenderTexture source, RenderTexture dest, Material material)
    {
        float off = 1.0f;
        Graphics.BlitMultiTap(source, dest, material,
            new Vector2(-off, -off),
            new Vector2(-off, off),
            new Vector2(off, off),
            new Vector2(off, -off)
        );
    }
}

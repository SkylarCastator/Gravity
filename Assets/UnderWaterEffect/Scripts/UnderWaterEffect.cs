using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))] 

public class UnderWaterEffect : MonoBehaviour{

	[Tooltip("The texture that will give the effect of drops on the screen")]
	public Texture dropsTexture;
	[Tooltip("The tag of the object that contains the collider(trigger), responsible for defining the water area")]
	public string waterTag = "waterTag";
	[Tooltip("The color of the water below the surface")]
	public Color waterColor = new Color32 (15, 150, 125, 0);
	[Range(0.1f,0.5f)][Tooltip("The size of the distortion that the image will receive under the water")]
	public float distortion = 0.2f;
	[Range(0.0f,0.3f)][Tooltip("The speed of the distortion that the image will receive under the water")]
	public float distortionSpeed = 0.2f;
	[Range(0.1f, 0.9f)][Tooltip("The intensity of the color of the water below the surface")]
	public float colorIntensity = 0.3f;
	[Range(0,5)][Tooltip("The distance of view under the water")]
	public int visibilityDistance = 2;
	[Range(0,10)][Tooltip("The visibility underwater")]
	public float visibility = 7;

	bool revert;
	bool revert2;
	bool underWater;
	bool cameOutOfTheWater;
	float timer1;
	float timer2;
	float timerDrops;
	GameObject quadDrops;

	int iterations = 3;
	float strengthX = 0.00f;
	float strengthY = 0.00f;
	float blurSpread = 0.6f;
	float angleVortex = 0;
	float edgesOnly = 0.0f;
	Color edgesOnlyBgColor = Color.white;
	Vector2 centerVortex = new Vector2(0.5f, 0.5f);
	Material materialBlur;
	Material edgeDetectMaterial = null;
	Material fisheyeMaterial = null;
	Material materialVortex;
	AudioSource audioSourceCamera;
	GameObject audioSourceUnderWater;
	[Space(15)][Tooltip("The sound that will be played when the player enters the water")]
	public AudioClip soundToEnter;
	[Tooltip("The sound that will be played when the player exits the water")]
	public AudioClip soundToExit;
	[Tooltip("The sound that will be played while the player is underwater")]
	public AudioClip underWaterSound;
	[Space(15)]
	[Tooltip("Shader 'SrBlur' must be associated with this variable")]
	public Shader SrBlur;
	[Tooltip("Shader 'SrEdge' must be associated with this variable")]
	public Shader SrEdge;
	[Tooltip("Shader 'SrFisheye' must be associated with this variable")]
	public Shader SrFisheye;
	[Tooltip("Shader 'SrVortex' must be associated with this variable")]
	public Shader SrVortex;
	[Tooltip("Shader 'SrQuad' must be associated with this variable")]
	public Shader SrQuad;

	void Awake (){
		if (materialVortex == null){
			materialVortex = new Material(SrVortex);
			materialVortex.hideFlags = HideFlags.HideAndDontSave;
		}
		if (materialBlur == null) {
			materialBlur = new Material(SrBlur);
			materialBlur.hideFlags = HideFlags.DontSave;
		}
			
		GetComponent<SphereCollider> ().radius = 0.005f;
		GetComponent<SphereCollider> ().isTrigger = false;
		GetComponent<Rigidbody> ().isKinematic = true;
		GetComponent<Camera> ().nearClipPlane = 0.01f;

		iterations = 5 - visibilityDistance;
		blurSpread = 1-(visibility/10);
		edgesOnly = colorIntensity;
		edgesOnlyBgColor = waterColor;

		quadDrops = GameObject.CreatePrimitive(PrimitiveType.Quad);
		Destroy (quadDrops.GetComponent<MeshCollider> ());
		quadDrops.transform.localScale = new Vector3 (0.16f, 0.16f, 1.0f);
		quadDrops.transform.parent = transform;
		quadDrops.transform.localPosition = new Vector3 (0, 0, 0.05f);
		quadDrops.transform.localEulerAngles = new Vector3 (0, 0, 0);
		quadDrops.GetComponent<Renderer>().material.shader = SrQuad;
		quadDrops.GetComponent<Renderer> ().material.SetTexture ("_BumpMap", dropsTexture);
		quadDrops.GetComponent<Renderer> ().material.SetFloat ("_BumpAmt", 0);

		audioSourceUnderWater = new GameObject ();
		audioSourceUnderWater.AddComponent (typeof(AudioSource));
		audioSourceUnderWater.GetComponent<AudioSource> ().loop = true;
		audioSourceUnderWater.transform.parent = transform;
		audioSourceUnderWater.transform.localPosition = new Vector3 (0, 0, 0);
		audioSourceUnderWater.GetComponent<AudioSource> ().clip = underWaterSound;
		audioSourceUnderWater.SetActive (false);

		audioSourceCamera = GetComponent<AudioSource> ();
		audioSourceCamera.playOnAwake = false;
		CheckSupport ();
	}

	void CheckSupport(){
		if (!SrBlur.isSupported) { Debug.LogError ("Shader 'SrBlur' not supported"); }
		if (!SrEdge.isSupported) { Debug.LogError ("Shader 'SrEdge' not supported"); }
		if (!SrFisheye.isSupported) { Debug.LogError ("Shader 'SrFisheye' not supported"); }
		if (!SrVortex.isSupported) { Debug.LogError ("Shader 'SrVortex' not supported"); }
		if (!SrQuad.isSupported) { Debug.LogError ("Shader 'SrQuad' not supported"); }
	}
		
	void OnDisable(){
		audioSourceUnderWater.SetActive (false);
		timerDrops = 0;
		cameOutOfTheWater = false;
		quadDrops.GetComponent<Renderer> ().material.SetFloat ("_BumpAmt", 0);
	}

	void Update (){
		if (revert == false)   { timer1 += Time.deltaTime*distortionSpeed; }
		if (timer1 > 0.5f) { revert = true;	}
		if (revert == true)    { timer1 -= Time.deltaTime*distortionSpeed; }
		if (timer1 < 0)    { revert = false; }
		if (revert2 == false)  { timer2 += Time.deltaTime*distortionSpeed*2; }
		if (timer2 > 2)   { revert2 = true; }
		if (revert2 == true)   { timer2 -= Time.deltaTime*distortionSpeed*2; }
		if (timer2 < -1)  { revert2 = false; }

		if (cameOutOfTheWater == true) {
			timerDrops -= Time.deltaTime*20;
			quadDrops.GetComponent<Renderer> ().material.SetTextureOffset ("_BumpMap", new Vector2 (0, -timerDrops/100));
			quadDrops.GetComponent<Renderer> ().material.SetFloat ("_BumpAmt", timerDrops);
			if(timerDrops < 0){
				timerDrops = 0;
				cameOutOfTheWater = false;
				quadDrops.GetComponent<Renderer> ().material.SetFloat ("_BumpAmt", 0);
			}
		}

		centerVortex = new Vector2(timer2,0.5f);
		angleVortex = ((timer1 * 20) - 10)*(distortion*2);
		strengthX = (timer1/2)*distortion;
		strengthY = 0.5f-timer1*distortion;
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination){
		if (underWater) {
			//effect1
			RenderTexture tmp1 = RenderTexture.GetTemporary (source.width / 2, source.height / 2);
			fisheyeMaterial = CheckShaderAndCreateMaterial (SrFisheye, fisheyeMaterial);
			float ar = (source.width) / (source.height);
			fisheyeMaterial.SetVector ("intensity", new Vector4 (strengthX * ar * 0.15625f, strengthY * 0.15625f, strengthX * ar * 0.15625f, strengthY * 0.15625f));
			Graphics.Blit (source, tmp1, fisheyeMaterial);
			//effect2
			RenderTexture tmp2 = RenderTexture.GetTemporary (tmp1.width, tmp1.height);
			RenderTexture.ReleaseTemporary (tmp1);
			edgeDetectMaterial = CheckShaderAndCreateMaterial (SrEdge, edgeDetectMaterial);
			edgeDetectMaterial.SetFloat ("_BgFade", edgesOnly);
			edgeDetectMaterial.SetFloat ("_SampleDistance", 0);
			edgeDetectMaterial.SetVector ("_BgColor", edgesOnlyBgColor);
			edgeDetectMaterial.SetFloat ("_Threshold", 0);
			Graphics.Blit (tmp1, tmp2, edgeDetectMaterial, 4);
			//effect3
			RenderTexture tmp3 = RenderTexture.GetTemporary (tmp2.width, tmp2.height);
			RenderTexture.ReleaseTemporary (tmp2);
			RenderDistortion (materialVortex, tmp2, tmp3, angleVortex, centerVortex, new Vector2 (1, 1));
			//effect4
			RenderTexture buffer = RenderTexture.GetTemporary (tmp3.width, tmp3.height, 0);
			DownSample4x (tmp3, buffer);
			for (int i = 0; i < iterations; i++) {
				RenderTexture buffer2 = RenderTexture.GetTemporary (tmp3.width, tmp3.height, 0);
				FourTapCone (buffer, buffer2, i);
				RenderTexture.ReleaseTemporary (buffer);
				buffer = buffer2;
			}
			Graphics.Blit (buffer, destination);
			RenderTexture.ReleaseTemporary (buffer);
			RenderTexture.ReleaseTemporary (tmp3);
		} else {
			Graphics.Blit (source, destination);
		}
	}

	void FourTapCone (RenderTexture source, RenderTexture dest, int iteration){
		float off = 0.5f + iteration*blurSpread;
		Graphics.BlitMultiTap (source, dest, materialBlur,
			new Vector2(-off, -off),
			new Vector2(-off,  off),
			new Vector2( off,  off),
			new Vector2( off, -off)
		);
	}

	void DownSample4x (RenderTexture source, RenderTexture dest){
		float off = 1.0f;
		Graphics.BlitMultiTap (source, dest, materialBlur,
			new Vector2(-off, -off),
			new Vector2(-off,  off),
			new Vector2( off,  off),
			new Vector2( off, -off)
		);
	}

	void RenderDistortion(Material material, RenderTexture source, RenderTexture destination, float angle, Vector2 center, Vector2 radius){
		bool invertY = source.texelSize.y < 0.0f;
		if (invertY){
			center.y = 1.0f - center.y;
			angle = -angle;
		}
		Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angle), Vector3.one);
		material.SetMatrix("_RotationMatrix", rotationMatrix);
		material.SetVector("_CenterRadius", new Vector4(center.x, center.y, radius.x, radius.y));
		material.SetFloat("_Angle", angle*Mathf.Deg2Rad);
		Graphics.Blit(source, destination, material);
	}

	Material CheckShaderAndCreateMaterial (Shader s, Material m2Create){
		if (s.isSupported && m2Create && m2Create.shader == s) {
			return m2Create;
		}
		m2Create = new Material (s);
		m2Create.hideFlags = HideFlags.DontSave;
		return m2Create;
	}

	void OnTriggerEnter (Collider colisor){
		if (colisor.gameObject.CompareTag (waterTag)) {
			underWater = true;
			cameOutOfTheWater = false;
			quadDrops.GetComponent<Renderer> ().material.SetFloat ("_BumpAmt", 0);
			audioSourceCamera.clip = soundToEnter;
			audioSourceCamera.PlayOneShot (audioSourceCamera.clip);
			audioSourceUnderWater.SetActive (true);
		}
	}

	void OnTriggerExit (Collider colisor){
		if (colisor.gameObject.CompareTag (waterTag)) {
			underWater = false;
			cameOutOfTheWater = true;
			timerDrops = 40;
			audioSourceCamera.clip = soundToExit;
			audioSourceCamera.PlayOneShot (audioSourceCamera.clip);
			audioSourceUnderWater.SetActive (false);
		}
	}
}
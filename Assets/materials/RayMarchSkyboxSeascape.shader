// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/RayMarchSkyboxSeascape" {
	Properties {
	        _UpVector ("Up Vector", Vector) = (0.0,1.0,0.0)
	        _RightVector ("Up Vector", Vector) = (1.0,0.0,0.0)
	        _ForwardVector ("Up Vector", Vector) = (0.0,0.0,1.0)
	        _MyWorldPosition ("My World Location", Vector) = (0.0,0.0,0.0)
	        _MyWorldCameraLook ("My World Camera Look", Vector) = (0.0,0.0,0.0)
			_SeaBaseColor ("Sea Base Color", Color) = (0.8,0.9,0.6,1.0)
			_SeaWaterColor ("Sea Water Color", Color) = (0.8,0.9,0.6, 1.0)
			_SkyColor ("Sky Color", Color) = (0.0,0.0,1.0,1.0)
	}
	SubShader {
		Pass {
            CGPROGRAM
            
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma target 3.0
            
            float4 _UpVector;
            float4 _RightVector;
            float4 _ForwardVector;
            float3 _MyWorldPosition;
            float3 _MyWorldCameraLookVector;
            float3 _MyWorldCameraLook;
			fixed4 _SeaBaseColor;
			fixed4 _SeaWaterColor;
			fixed4 _SkyColor;
            
            // Quality metric between 0 and 1 that is computed at the start of frag shader based on the angle
            // between the world look direction and the world ray for that pixel
            float fovQuality = 1.0;
            
            static const int NUM_STEPS = 5;
			static const float PI	 	= 3.1415;
			static const float EPSILON	= 1e-3;
			float EPSILON_NRM	= 0.01 / 1024;

			// sky
			static const float SKY_X = _SkyColor.rgb.r;
			static const float SKY_Y = _SkyColor.rgb.g;
			static const float SKY_Z = _SkyColor.rgb.b;

			// sea
			static const int ITER_GEOMETRY_LOW  = 2;
			static const int ITER_GEOMETRY_HIGH = 3;
			
			static const int ITER_FRAGMENT_LOW  = 4;
			static const int ITER_FRAGMENT_HIGH = 5;
			
			static const float SEA_HEIGHT = 0.6;
			static const float SEA_CHOPPY = 4.0;
			static const float SEA_SPEED = 0.8;
			static const float SEA_FREQ = 0.16;
			static const float3 SEA_BASE = _SeaBaseColor.rgb;
			static const float3 SEA_WATER_COLOR = _SeaWaterColor.rgb;
			float SEA_TIME = 0;
			static const float2x2 octave_m = float2x2(1.6,1.2,-1.2,1.6);

            float fract(float x) {
            	return x - floor(x);
            }
            
            float2 fract(float2 x) {
            	return x - floor(x);
            }
            
            float hash( float2 p ) {
				float h = dot(p,float2(127.1,311.7));	
			    return fract(sin(h)*43758.5453123);
			}
			
			// TESTED
			float noise( in float2 p ) {
			    float2 i = floor( p );
			    float2 f = fract( p );	
				float2 u = f*f*(3.0-2.0*f);
				

				//return fract(p).x;
				
			    return -1.0+2.0*lerp( lerp( hash( i + float2(0.0,0.0) ), 
			                     hash( i + float2(1.0,0.0) ), u.x),
			                lerp( hash( i + float2(0.0,1.0) ), 
			                     hash( i + float2(1.0,1.0) ), u.x), u.y);
			}

			// lighting
			float diffuse(float3 n,float3 l,float p) {
			    return pow(dot(n,l) * 0.4 + 0.6,p);
			}
			float specular(float3 n,float3 l,float3 e,float s) {    
			    float nrm = (s + 8.0) / (3.1415 * 8.0);
			    return pow(max(dot(reflect(e,n),l),0.0),s) * nrm;
			}

			// sky
			float3 getSkyColor(float3 e) {
				//return _SkyColor.rgb;
			    e.y = max(e.y,0.0);
			    float3 ret;
				ret.x = pow(1.0-e.y,2.0);
			    ret.y = 1.0-e.y;
			    ret.z = 0.6+(1.0-e.y)*0.4;
			    return ret;
			}

			/*
				ret.x = pow(1.0-e.y,2.0);
			    ret.y = 1.0-e.y;
			    ret.z = 0.6+(1.0-e.y)*0.4;
			*/


			// TESTED
			// sea
			float sea_octave(float2 uv, float choppy) {
			    uv += noise(uv);  
			    float2 wv = 1.0-abs(sin(uv));
			    float2 swv = abs(cos(uv));    
			    wv = lerp(wv,swv,wv);
			    return pow(1.0-pow(wv.x * wv.y,0.65),choppy);
			}
			
			float map(float3 p) {
			    float freq = SEA_FREQ;
			    float amp = SEA_HEIGHT;
			    float choppy = SEA_CHOPPY;
			    float2 uv = p.xz; uv.x *= 0.75;

				// CHECKPOINT - TESTED TO HERE
				//return uv.x + uv.y;
               // return p.y;

			    float d = 0.0, h = 0.0;  
			   // p = float3(0.0, 0.0, 0.0);
			    //return 0.4f;  
			    // int iters = ITER_GEOMETRY_LOW + ceil(fovQuality * (ITER_GEOMETRY_HIGH - ITER_GEOMETRY_LOW));
			    for(int i = 0; i < ITER_GEOMETRY_HIGH; i++) { // ITER_GEOMETRY
			    	//d = 19.0; // remove  
			    	
			    	//if (i == 2) {
			    	//return 0.4f;
			    	            
			    	d = sea_octave((uv+SEA_TIME)*freq,choppy);
			    	d += sea_octave((uv-SEA_TIME)*freq,choppy);

			        h += d * amp;   
			                  
			    	uv = mul(octave_m, uv); 
			    	
			    	freq *= 1.9; 
			    	amp *= 0.22;
			        choppy = lerp(choppy,1.0,0.2);
			    }

			    return p.y - h;
			}

			float map_detailed(float3 p) {
			    float freq = SEA_FREQ;
			    float amp = SEA_HEIGHT;
			    float choppy = SEA_CHOPPY;
			    float2 uv = p.xz; 
			    uv.x *= 0.75;
			    
			    float d, h = 0.0;    
			    
			    int iters = ITER_FRAGMENT_LOW + ceil(fovQuality * (ITER_FRAGMENT_HIGH - ITER_FRAGMENT_LOW));
			    for(int i = 0; i < iters; i++) {        
			    	d = sea_octave((uv+SEA_TIME)*freq,choppy);
			    	d += sea_octave((uv-SEA_TIME)*freq,choppy);
			        h += d * amp;        
			    	uv = mul(octave_m, uv); 
			    	freq *= 1.9; 
			    	amp *= 0.22;
			        choppy = lerp(choppy,1.0,0.2);
			    }
			    return p.y - h;
			}
			
			float3 getSeaColor(float3 p, float3 n, float3 l, float3 eye, float3 dist) {  
			    float fresnel = 1.0 - max(dot(n,-eye),0.0);
			    fresnel = pow(fresnel,3.0) * 0.65;
			        
			    float3 reflected = getSkyColor(reflect(eye,n));    
			    float3 refractted = SEA_BASE + diffuse(n,l,80.0) * SEA_WATER_COLOR * 0.12; 
			    
			    float3 color_ = lerp(refractted,reflected,fresnel);
			    
			    float atten = max(1.0 - dot(dist,dist) * 0.001, 0.0);
			    color_ += SEA_WATER_COLOR * (p.y - SEA_HEIGHT) * 0.18 * atten;
			    
			    float c = specular(n,l,eye,60.0);
			    color_ += float3(c, c, c);
			    
			    return color_;
			}

			// tracing
			float3 getNormal(float3 p, float eps) {
			    float3 n;
			    n.y = map_detailed(p);    
			    n.x = map_detailed(float3(p.x+eps,p.y,p.z)) - n.y;
			    n.z = map_detailed(float3(p.x,p.y,p.z+eps)) - n.y;
			    n.y = eps;
			    return normalize(n);
			}

			float heightMapTracing(float3 ori, float3 dir, out float3 p) { 
				p = float3(0.0, 0.0, 0.0); 
				
			    float tm = 0.0;
			    float tx = 1000.0;    
			    float hx = map(ori + dir * tx);
			    if(hx > 0.0) return tx;   
			    float hm = map(ori + dir * tm);    
			    float tmid = 0.0;
			    
			    for(int i = 0; i < NUM_STEPS; i++) {
			        tmid = lerp(tm,tx, hm/(hm-hx));                   
			        p = ori + dir * tmid;                   
			    	float hmid = map(p);
					if(hmid < 0.0) {
			        	tx = tmid;
			            hx = hmid;
			        } else {
			            tm = tmid;
			            hm = hmid;
			        }
			    }
			    return tmid;
			}

            
            struct v2f {
                float4 position : SV_POSITION;
                //float2 uv : TEXCOORD0; // stores uv
                float3 worldSpacePosition : TEXCOORD0;
                float3 worldSpaceView : TEXCOORD1; 
            };
            
            v2f vert(appdata_full i) {
            	SEA_TIME = 0; // TODO: swap above
            
                v2f o;
                o.position = UnityObjectToClipPos (i.vertex);
                
                float4 vertexWorld = mul(unity_ObjectToWorld, i.vertex);
                
                o.worldSpacePosition = vertexWorld.xyz;
                o.worldSpaceView = vertexWorld.xyz - _WorldSpaceCameraPos;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
           		EPSILON_NRM	= 0.35 / _ScreenParams.x;
           		SEA_TIME = _Time[1] * SEA_SPEED;
            
				float3 ro = _WorldSpaceCameraPos;
				//float3 ro = float3(0.0, 3.5, 0.0); // TODO: remove
				
				float3 rd = normalize(i.worldSpaceView);
			
				// VR Quality based on angular distance from look direction
				fovQuality = dot(_MyWorldCameraLook, rd);
				fovQuality = pow(fovQuality, 6);
						
				// tracing
			    float3 p = float3(5.0,233.0,1.0);
			    heightMapTracing(ro,rd,p);
			    float3 dist = p - ro;
			    
			    // Increase epsilon for lower quality pixels
			    float fovQualityNormalEpsilonFactor = 1.0 + saturate(0.8 - fovQuality);
			    fovQualityNormalEpsilonFactor = dot(fovQualityNormalEpsilonFactor, fovQualityNormalEpsilonFactor);
			    
			    fovQualityNormalEpsilonFactor = 1.0;
			    float distToPixelWorldSquared = dot(dist, dist);
			    
			    float3 n = getNormal(p, distToPixelWorldSquared * EPSILON_NRM * fovQualityNormalEpsilonFactor);
			    
			    float3 light = normalize(float3(sin(SEA_TIME * 0.03f),1.0,cos(SEA_TIME * 0.03f))); 
			            
			    //return float4(p.x, p.y, p.z, 1.0);
			               
			    // color
			    float3 color_ = lerp(
			        getSkyColor(rd),
			        getSeaColor(p,n,light,rd,dist),
			    	pow(smoothstep(0.0,-0.05,rd.y),0.3));
			    		

				//float3 c = getSkyColor(rd);
				//return float4(c.x, c.y, c.z, 1.0);
				
			    //return float4(p.x, p.y, p.z, 1.0);
			    //float2 f = p.xy;
			    //float n2 = noise(f);
			    //return float4(n2, n2, n2, 1.0);
			    //float2 n3 = float2(5.1, 4.3);
			    //float n2 = n3 - floor (n3);
			    //float n2 = noise(n3);
			    
			    //p = ro + rd * 50.0;
			  
			    //float j = map(ro + rd * 50.0);
			    //float j = map_detailed(ro + rd * 50.0);
			   //float j = sea_octave((ro + rd * 50.0).xz, 4.0);
			    //return float4(j, j, j, 1.0);
			  //  return float4(noise(rd.xy), noise(rd.xz), noise(rd.yz), 1.0);
			   // return float4(rd.x, rd.y, rd.z, 1.0);
					        	        
			    float3 finalColor = pow(color_,float3(0.75, 0.75, 0.75));
			    
			    // !! Visualize field of view quality falloff raw
			    // finalColor = float3(fovQuality, fovQuality, fovQuality);
			    
			    // !! Visualize field of view quality falloff blended
			    // finalColor = lerp(finalColor, float3(1.0f, 0.0f, 0.0f), fovQuality);
			    
			    return float4(finalColor.x, finalColor.y, finalColor.z, 1.0);

            }

            ENDCG
		}
	}
}

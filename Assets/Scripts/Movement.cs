using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{   
    [SerializeField] InputAction thrust;
    [SerializeField] InputAction rotation;
    [SerializeField] float thrustStrength = 100f;
    [SerializeField] float rotationStrength = 100f;
    [SerializeField] AudioClip mainEngineSFX;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem[] rightThrustParticles;
    [SerializeField] ParticleSystem[] leftThrustParticles;

    Rigidbody rb;
    AudioSource audioSource;

    private void Start() 
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();    
    }

    private void OnEnable() 
    {
        thrust.Enable(); 
        rotation.Enable();
    }

    private void FixedUpdate()
    {
        ProcessThrust();
        ProcessRotation();
    }

    private void ProcessThrust()
    {
        if (thrust.IsPressed())
        {
            StartThrusting();
        }
        else
        {
            StopThrusting();
        }
    }

    private void StartThrusting()
    {
        rb.AddRelativeForce(Vector3.up * thrustStrength * Time.fixedDeltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngineSFX);
        }
        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
    }

    private void StopThrusting()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ProcessRotation()
    {
        float rotationInput = rotation.ReadValue<float>();
        if(rotationInput < 0)
        {
            RotateRight();
        }
        else if(rotationInput > 0)
        {
            RotateLeft();
        }
        else
        {
            StopRotating();
        }
    }

    private void RotateRight()
    {
        ApplyRotation(rotationStrength);

        for(int i = 0; i < rightThrustParticles.Length; i++)
        {
            if (!rightThrustParticles[i].isPlaying)
            {
                leftThrustParticles[i].Stop();
                rightThrustParticles[i].Play();
            }
        }
    }

    private void RotateLeft()
    {
        ApplyRotation(-rotationStrength);

        for (int i = 0; i < leftThrustParticles.Length; i++)
        {
            if (!leftThrustParticles[i].isPlaying)
            {
                rightThrustParticles[i].Stop();
                leftThrustParticles[i].Play();
            }
        }
    }

    private void StopRotating()
    {
        for (int i = 0; i < rightThrustParticles.Length; i++)
        {
            rightThrustParticles[i].Stop();
            leftThrustParticles[i].Stop();
        }
    }

    private void ApplyRotation(float rotationThisFrame)
    {
        rb.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.fixedDeltaTime);
        rb.freezeRotation = false;
    }
}

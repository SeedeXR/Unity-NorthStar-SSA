using UnityEngine;
using Meta.Utilities.Environment;

/// <summary>
/// Controls a platform that descends from an island surface to the underwater seabed.
/// Attach to the lift platform GameObject. Player stands on the platform and triggers descent.
/// Automatically swaps EnvironmentProfile when crossing the water surface.
/// </summary>
public class UnderwaterLiftController : MonoBehaviour
{
    [Header("Lift Settings")]
    [SerializeField] private float m_surfaceHeight = 5f;
    [SerializeField] private float m_underwaterDepth = -30f;
    [SerializeField] private float m_speed = 3f;
    [SerializeField] private AnimationCurve m_speedCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Environment Profiles")]
    [SerializeField] private EnvironmentProfile m_aboveWaterProfile;
    [SerializeField] private EnvironmentProfile m_underwaterProfile;

    [Header("Underwater Controller")]
    [SerializeField] private UnderwaterEnvironmentController m_underwaterController;

    [Header("References")]
    [SerializeField] private Transform m_playerTransform;

    private float m_journeyProgress;
    private bool m_isDescending;
    private bool m_isAscending;
    private bool m_isUnderwater;
    private float m_waterSurfaceY = 0f;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, m_surfaceHeight, transform.position.z);

        if (m_underwaterController != null)
            m_underwaterController.enabled = false;
    }

    /// <summary>
    /// Call this to start the descent from surface to underwater.
    /// </summary>
    public void StartDescent()
    {
        if (m_isDescending || m_isAscending) return;
        m_isDescending = true;
        m_isAscending = false;
        m_journeyProgress = 0f;
    }

    /// <summary>
    /// Call this to start ascending back to the surface.
    /// </summary>
    public void StartAscent()
    {
        if (m_isDescending || m_isAscending) return;
        m_isAscending = true;
        m_isDescending = false;
        m_journeyProgress = 0f;
    }

    private void Update()
    {
        if (!m_isDescending && !m_isAscending) return;

        float totalDistance = Mathf.Abs(m_surfaceHeight - m_underwaterDepth);
        m_journeyProgress += (m_speed / totalDistance) * Time.deltaTime;
        m_journeyProgress = Mathf.Clamp01(m_journeyProgress);

        float curvedProgress = m_speedCurve.Evaluate(m_journeyProgress);

        float fromY, toY;
        if (m_isDescending)
        {
            fromY = m_surfaceHeight;
            toY = m_underwaterDepth;
        }
        else
        {
            fromY = m_underwaterDepth;
            toY = m_surfaceHeight;
        }

        float newY = Mathf.Lerp(fromY, toY, curvedProgress);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Check water surface crossing
        bool nowUnderwater = transform.position.y < m_waterSurfaceY;
        if (nowUnderwater != m_isUnderwater)
        {
            m_isUnderwater = nowUnderwater;
            OnCrossWaterSurface(m_isUnderwater);
        }

        // Journey complete
        if (m_journeyProgress >= 1f)
        {
            m_isDescending = false;
            m_isAscending = false;
        }
    }

    private void OnCrossWaterSurface(bool goingUnderwater)
    {
        var envSystem = EnvironmentSystem.Instance;
        if (envSystem == null) return;

        if (goingUnderwater)
        {
            if (m_underwaterProfile != null)
                envSystem.SetProfile(m_underwaterProfile);

            if (m_underwaterController != null)
                m_underwaterController.enabled = true;
        }
        else
        {
            if (m_aboveWaterProfile != null)
                envSystem.SetProfile(m_aboveWaterProfile);

            if (m_underwaterController != null)
                m_underwaterController.enabled = false;
        }
    }

    // Parent player to platform when they step on
    private void OnTriggerEnter(Collider other)
    {
        if (m_playerTransform != null && other.transform == m_playerTransform)
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_playerTransform != null && other.transform == m_playerTransform)
        {
            other.transform.SetParent(null);
        }
    }
}

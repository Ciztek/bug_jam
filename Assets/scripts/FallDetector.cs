using UnityEngine;

public class FallDetector : MonoBehaviour
{
    [Header("Fall Detection")]
    [Tooltip("Hauteur en dessous de laquelle le joueur est considere comme tombe")]
    public float fallThreshold = -10f;

    [Tooltip("Hauteur a laquelle teleporter le joueur apres une chute")]
    public float respawnHeight = 50f;

    [Tooltip("Delai avant de teleporter le joueur (pour laisser le temps au son de jouer)")]
    public float respawnDelay = 1.5f;

    [Header("References")]
    [Tooltip("Le joueur")]
    public Transform player;

    private Vector3 lastSafePosition;
    private bool isFalling = false;
    private AudioPlayerManager audioManager;

    void Start()
    {
        // Trouver le joueur automatiquement si non assigne
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("FallDetector: Aucun joueur trouve avec le tag 'Player'!");
                enabled = false;
                return;
            }
        }

        // Trouver l'AudioPlayerManager
        audioManager = FindObjectOfType<AudioPlayerManager>();
        if (audioManager == null)
        {
            Debug.LogWarning("FallDetector: AudioPlayerManager non trouve! Le son du scorpion ne sera pas joue.");
        }

        // Initialiser la position de securite
        lastSafePosition = player.position;
    }

    void Update()
    {
        if (player == null)
            return;

        // Verifier si le joueur est tombe
        if (player.position.y < fallThreshold && !isFalling)
        {
            isFalling = true;
            OnPlayerFell();
        }
        else if (player.position.y >= fallThreshold && !isFalling)
        {
            // Mise a jour de la derniere position sure (uniquement en X et Z)
            lastSafePosition = new Vector3(player.position.x, player.position.y, player.position.z);
        }
    }

    private void OnPlayerFell()
    {
        Debug.Log("Le joueur est tombe de la ville!");

        // Jouer le son du scorpion
        if (audioManager != null)
        {
            audioManager.PlayScorpion();
        }

        // Teleporter le joueur apres un delai
        Invoke("RespawnPlayer", respawnDelay);
    }

    private void RespawnPlayer()
    {
        if (player == null)
            return;

        // Teleporter le joueur au-dessus de sa derniere position (X, Z) mais a une hauteur fixe
        Vector3 respawnPosition = new Vector3(lastSafePosition.x, respawnHeight, lastSafePosition.z);
        player.position = respawnPosition;

        // Reinitialiser le CharacterController si present (pour eviter des bugs de physique)
        CharacterController characterController = player.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
            characterController.enabled = true;
        }

        // Reinitialiser le Rigidbody si present (pour arreter la vitesse de chute)
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log($"Joueur teleporte a: {respawnPosition}");
        
        isFalling = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Dessiner le seuil de chute dans l'editeur
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-1000, fallThreshold, -1000), new Vector3(1000, fallThreshold, -1000));
        Gizmos.DrawLine(new Vector3(-1000, fallThreshold, 1000), new Vector3(1000, fallThreshold, 1000));
        Gizmos.DrawLine(new Vector3(-1000, fallThreshold, -1000), new Vector3(-1000, fallThreshold, 1000));
        Gizmos.DrawLine(new Vector3(1000, fallThreshold, -1000), new Vector3(1000, fallThreshold, 1000));

        // Dessiner la hauteur de respawn
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-1000, respawnHeight, -1000), new Vector3(1000, respawnHeight, -1000));
        Gizmos.DrawLine(new Vector3(-1000, respawnHeight, 1000), new Vector3(1000, respawnHeight, 1000));
        Gizmos.DrawLine(new Vector3(-1000, respawnHeight, -1000), new Vector3(-1000, respawnHeight, 1000));
        Gizmos.DrawLine(new Vector3(1000, respawnHeight, -1000), new Vector3(1000, respawnHeight, 1000));
    }
}

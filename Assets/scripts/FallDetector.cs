using UnityEngine;

public class FallDetector : MonoBehaviour
{
    [Header("Fall Detection")]
    [Tooltip("Hauteur en dessous de laquelle le joueur est considere comme tombe")]
    public float fallThreshold = -10f;

    [Tooltip("Distance maximale pour chercher le sol au-dessus de la derniere position")]
    public float maxRaycastDistance = 200f;

    [Tooltip("Hauteur au-dessus du sol pour teleporter le joueur")]
    public float heightAboveGround = 2f;

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

        // Chercher le sol en lancant un raycast depuis le haut vers le bas a la derniere position sure
        Vector3 raycastStart = new Vector3(lastSafePosition.x, lastSafePosition.y + maxRaycastDistance, lastSafePosition.z);
        RaycastHit hit;

        Vector3 respawnPosition;

        // Lancer un raycast vers le bas pour trouver le sol
        if (Physics.Raycast(raycastStart, Vector3.down, out hit, maxRaycastDistance * 2))
        {
            // Teleporter le joueur legerement au-dessus du sol trouve
            respawnPosition = hit.point + Vector3.up * heightAboveGround;
            Debug.Log($"Sol trouve a: {hit.point}, teleportation a: {respawnPosition}");
        }
        else
        {
            // Si aucun sol n'est trouve, utiliser la derniere position sure + une hauteur fixe
            respawnPosition = lastSafePosition + Vector3.up * 5f;
            Debug.LogWarning($"Aucun sol trouve, utilisation de la derniere position sure: {respawnPosition}");
        }

        // Desactiver temporairement le CharacterController pour la teleportation
        CharacterController characterController = player.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // Teleporter le joueur
        player.position = respawnPosition;

        // Reactiver le CharacterController
        if (characterController != null)
        {
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

        // Dessiner la derniere position sure si disponible
        if (Application.isPlaying && lastSafePosition != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(lastSafePosition, 1f);
            
            // Dessiner le raycast qui sera utilise pour trouver le sol
            Gizmos.color = Color.yellow;
            Vector3 rayStart = new Vector3(lastSafePosition.x, lastSafePosition.y + maxRaycastDistance, lastSafePosition.z);
            Gizmos.DrawLine(rayStart, rayStart + Vector3.down * maxRaycastDistance * 2);
        }
    }
}

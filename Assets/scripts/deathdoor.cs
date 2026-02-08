using UnityEngine;
using UnityEngine.InputSystem;

public class deathdoor : MonoBehaviour
{
    [Header("Death Door Settings")]
    [Tooltip("Afficher un message avant de quitter")]
    public bool showMessage = true;

    [Tooltip("Message affiche avant de quitter le jeu")]
    public string deathMessage = "GAME OVER - Tu es mort !";

    [Tooltip("Temps d'attente avant de quitter (secondes)")]
    public float delayBeforeQuit = 1f;

    [Header("Interaction Settings")]
    [Tooltip("Distance pour interagir avec la porte")]
    public float interactDistance = 3f;

    [Tooltip("Hauteur du texte au-dessus de la porte")]
    public float textHeight = 2.5f;

    [Tooltip("Taille du texte")]
    public float textSize = 0.1f;

    [Header("References")]
    [Tooltip("Le joueur (avec PlayerController)")]
    public Transform player;

    private bool hasTriggered = false;
    private bool isPlayerNear = false;
    private TextMesh hintText;
    private PlayerInput playerInput;
    private InputAction interactAction;
    private Camera mainCamera;

    void Start()
    {
        // Si le joueur n'est pas assigne, le chercher
        mainCamera = Camera.main;
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Recuperer le PlayerInput du joueur
        if (player != null)
        {
            playerInput = player.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                interactAction = playerInput.actions["Interact"];
            }
        }

        // Creer le texte flottant
        hintText = CreateFloatingText();
        hintText.text = "";
    }

    void Update()
    {
        if (hasTriggered || player == null)
            return;

        // Faire tourner le texte vers la camera
        if (hintText != null && Camera.main != null)
        {
            hintText.transform.rotation = Quaternion.LookRotation(hintText.transform.position - Camera.main.transform.position);
        }

        // Calculer la distance au joueur
        float distance = Vector3.Distance(transform.position, player.position);

        // Le joueur est proche
        if (distance <= interactDistance)
        {
            if (!isPlayerNear)
            {
                isPlayerNear = true;
                hintText.text = "[E] Open the door";
            }

            // Detecter l'appui sur E
            if (interactAction != null && interactAction.WasPressedThisFrame())
            {
                OpenDoor();
            }
        }
        else
        {
            if (isPlayerNear)
            {
                isPlayerNear = false;
                hintText.text = "";
            }
        }
    }

    private void OpenDoor()
    {
        mainCamera.GetComponent<AudioPlayerManager>().PlayGameOver();
        hasTriggered = true;
        hintText.text = "";
        TriggerDeath();
    }

    private void TriggerDeath()
    {
        if (showMessage)
        {
            Debug.LogError(deathMessage);
        }

        // Quitter le jeu apres un delai
        Invoke("QuitGame", delayBeforeQuit);
    }

    private void QuitGame()
    {
        Debug.Log("Le jeu se ferme...");

        #if UNITY_EDITOR
            // En mode editeur Unity, arreter le mode Play
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // En build, quitter l'application
            Application.Quit();
        #endif
    }

    private TextMesh CreateFloatingText()
    {
        GameObject textObj = new GameObject("DoorHintText");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = new Vector3(0, textHeight, 0);

        TextMesh tm = textObj.AddComponent<TextMesh>();
        tm.characterSize = textSize;
        tm.fontSize = 100;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.white;
        tm.text = "";

        return tm;
    }

    private void OnDrawGizmosSelected()
    {
        // Dessiner la zone d'interaction dans l'editeur
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}

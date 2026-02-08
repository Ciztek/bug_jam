using UnityEngine;
using UnityEngine.InputSystem;

public class BaguetteQuest : MonoBehaviour
{
    [Header("Personnages")]
    [Tooltip("Le GameObject du PNJ squalala (celui qui donne la quete)")]
    public Transform squalala;

    [Tooltip("Le GameObject du PNJ squalalabaker (le boulanger)")]
    public Transform squalalabaker;

    [Header("Objets")]
    [Tooltip("Le GameObject de la baguette dans la scene")]
    public GameObject baguette;

    [Header("Joueur")]
    [Tooltip("Le GameObject du joueur (avec PlayerController)")]
    public Transform player;

    [Header("Reglages")]
    [Tooltip("Distance pour interagir avec un PNJ")]
    public float interactDistance = 3f;

    [Tooltip("Hauteur du texte au-dessus de la tete")]
    public float textHeight = 2.5f;

    [Tooltip("Taille du texte")]
    public float textSize = 0.1f;

    // Etapes de la quete
    private enum QuestStep
    {
        NotStarted,        // Le joueur n'a pas encore parle a squalala
        GoToBaker,          // Le joueur doit aller voir le boulanger
        ReturnToSqualala,   // Le joueur a la baguette, doit revenir
        Finished            // Quete terminee
    }

    private QuestStep currentStep = QuestStep.NotStarted;

    // TextMesh pour afficher du texte au-dessus des tetes
    private TextMesh squalalaText;
    private TextMesh bakerText;

    // Pour afficher "Appuie sur E"
    private TextMesh squalalaHintText;
    private TextMesh bakerHintText;

    private float dialogTimer = 0f;
    private float dialogDuration = 4f;

    // Input System - reference au PlayerInput du joueur
    private PlayerInput playerInput;
    private InputAction interactAction;

    void Start()
    {
        // Recuperer le PlayerInput du joueur
        playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            interactAction = playerInput.actions["Interact"];
        }
        else
        {
            Debug.LogError("BaguetteQuest: Le joueur n'a pas de PlayerInput component!");
        }

        squalalaText = CreateFloatingText(squalala, "SqualalaDialogue", textHeight + 0.4f);
        bakerText = CreateFloatingText(squalalabaker, "BakerDialogue", textHeight + 0.4f);
        squalalaHintText = CreateFloatingText(squalala, "SqualalaHint", textHeight);
        bakerHintText = CreateFloatingText(squalalabaker, "BakerHint", textHeight);

        squalalaText.text = "";
        bakerText.text = "";
        squalalaHintText.text = "";
        bakerHintText.text = "";

        squalalaHintText.text = "[E] Parler";
    }

    void Update()
    {
        // Faire tourner les textes vers la camera
        FaceCamera(squalalaText.transform);
        FaceCamera(bakerText.transform);
        FaceCamera(squalalaHintText.transform);
        FaceCamera(bakerHintText.transform);

        // Timer pour effacer les dialogues apres un moment
        if (dialogTimer > 0f)
        {
            dialogTimer -= Time.deltaTime;
        }

        float distToSqualala = Vector3.Distance(player.position, squalala.position);
        float distToBaker = Vector3.Distance(player.position, squalalabaker.position);

        switch (currentStep)
        {
            case QuestStep.NotStarted:
                HandleNotStarted(distToSqualala);
                break;

            case QuestStep.GoToBaker:
                HandleGoToBaker(distToBaker);
                break;

            case QuestStep.ReturnToSqualala:
                HandleReturnToSqualala(distToSqualala);
                break;

            case QuestStep.Finished:
                break;
        }
    }

    private bool IsInteractPressed()
    {
        return interactAction != null && interactAction.WasPressedThisFrame();
    }

    private void HandleNotStarted(float distToSqualala)
    {
        // Afficher hint quand le joueur est proche
        if (distToSqualala <= interactDistance)
        {
            squalalaHintText.text = "[E] Parler";

            if (IsInteractPressed())
            {
                // Squalala parle
                squalalaText.text = "J'ai besoin de toi, va a la\nboulangerie me chercher\nune baguette de pain.";
                squalalaHintText.text = "";
                currentStep = QuestStep.GoToBaker;
                dialogTimer = dialogDuration;
                Camera.main.GetComponent<AudioPlayerManager>().PlaySqualala();

                // Afficher hint chez le baker
                bakerHintText.text = "[E] Prendre la baguette";
            }
        }
        else
        {
            squalalaHintText.text = "";
        }
    }

    private void HandleGoToBaker(float distToBaker)
    {
        // Effacer le dialogue de squalala apres un moment
        if (dialogTimer <= 0f && squalalaText.text != "")
        {
            squalalaText.text = "";
        }

        // Afficher hint quand le joueur est proche du baker
        if (distToBaker <= interactDistance)
        {
            bakerHintText.text = "[E] Prendre la baguette";

            if (IsInteractPressed())
            {
                // La baguette disparait
                if (baguette != null)
                    baguette.SetActive(false);

                // Le baker parle
                bakerText.text = "Tu m'as trouve, donne\ncette baguette a l'autre.";
                bakerHintText.text = "";
                currentStep = QuestStep.ReturnToSqualala;
                dialogTimer = dialogDuration;
                Camera.main.GetComponent<AudioPlayerManager>().PlaySqualala();
                // Afficher hint chez squalala
                squalalaHintText.text = "[E] Donner la baguette";
            }
        }
        else
        {
            bakerHintText.text = "";
        }
    }

    private void HandleReturnToSqualala(float distToSqualala)
    {
        // Effacer le dialogue du baker apres un moment
        if (dialogTimer <= 0f && bakerText.text != "")
        {
            bakerText.text = "";
        }

        // Afficher hint quand le joueur est proche de squalala
        if (distToSqualala <= interactDistance)
        {
            squalalaHintText.text = "[E] Donner la baguette";

            if (IsInteractPressed())
            {
                // Squalala parle
                squalalaText.text = "Merci connard";
                squalalaHintText.text = "";
                currentStep = QuestStep.Finished;
                dialogTimer = dialogDuration;

                // Nettoyer apres quelques secondes
                Invoke("CleanupTexts", dialogDuration);
            }
        }
        else
        {
            squalalaHintText.text = "";
        }
    }

    private void CleanupTexts()
    {
        squalalaText.text = "";
        bakerText.text = "";
        squalalaHintText.text = "";
        bakerHintText.text = "";
    }

    private TextMesh CreateFloatingText(Transform parent, string name, float height)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);
        textObj.transform.localPosition = new Vector3(0, height, 0);

        TextMesh tm = textObj.AddComponent<TextMesh>();
        tm.characterSize = textSize;
        tm.fontSize = 100;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.white;
        tm.text = "";

        return tm;
    }

    private void FaceCamera(Transform t)
    {
        if (Camera.main != null)
        {
            t.rotation = Quaternion.LookRotation(t.position - Camera.main.transform.position);
        }
    }
}

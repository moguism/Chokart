using TMPro;
using UnityEngine;

public class VerticalMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject menus;

    [SerializeField]
    private GameObject friendlist;
    public GameObject requestsList;

    [SerializeField]
    private GameObject friendPrefab;

    [SerializeField]
    private bool canInvite = false;

    private bool hasSetUp = false;

    private void Update()
    {
        if(!hasSetUp)
        {
            if(AuthManager.user == null)
            {
                return;
            }

            ManageFriendList();
            hasSetUp = true;
        }
    }

    public void ToggleAvailability()
    {
        menus.SetActive(!menus.activeInHierarchy);
    }

    public void ManageFriendList()
    {
        foreach (Friendship friendship in AuthManager.user.friendships)
        {
            ConfigureFriendship(friendlist.transform, friendship.senderUser ?? friendship.receiverUser, true);
        }
    }

    private void ConfigureFriendship(Transform parent, UserDto senderOrReceiver, bool isFriendList)
    {
        var friend = Instantiate(friendPrefab, parent);
        friend.GetComponentInChildren<TMP_Text>().text = senderOrReceiver.nickname;

        var friendScript = friend.GetComponent<FriendPrefab>();
        friendScript.friend = senderOrReceiver;

        if (isFriendList)
        {
            friendScript.SetAvailability(false, true);
        }
        else
        {
            // En este caso se trata de una solicitud
            friendScript.SetAvailability(true, false);
        }
    }
}

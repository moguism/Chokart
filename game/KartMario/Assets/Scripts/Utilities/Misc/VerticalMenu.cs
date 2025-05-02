using Injecta;
using System.Linq;
using System.Threading.Tasks;
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

    [Inject]
    private AuthManager authManager;

    private bool hasSetUp = false;

    private void Update()
    {
        if(!hasSetUp)
        {
            if(AuthManager.user == null)
            {
                return;
            }

            ManageFriendList(false);
            hasSetUp = true;
        }
    }

    public void ToggleAvailability()
    {
        menus.SetActive(!menus.activeInHierarchy);
    }

    public async Task RefreshFriendList()
    {
        await authManager.GetUserAsync(AuthManager.user.id, AuthManager.token);
        ManageFriendList(true);
    }

    private void ManageFriendList(bool deleteExisting)
    {
        if(deleteExisting)
        {
            FriendPrefab[] friendPrefabs = friendlist.GetComponentsInChildren<FriendPrefab>();
            for(int i = 0; i < friendPrefabs.Length; i++)
            {
                Destroy(friendPrefabs.ElementAt(i).gameObject);
            }
        }

        foreach (Friendship friendship in AuthManager.user.friendships)
        {
            UserDto user = friendship.senderUser.id != 0 ? friendship.senderUser : friendship.receiverUser;
            ConfigureFriendship(friendlist.transform, user, true, null);
        }
    }

    public void ConfigureFriendship(Transform parent, UserDto senderOrReceiver, bool isFriendList, string lobbyCodeToJoin)
    {
        //Debug.LogError(senderOrReceiver.nickname);

        var friend = Instantiate(friendPrefab, parent);
        friend.GetComponentInChildren<TMP_Text>().text = senderOrReceiver.nickname;

        var friendScript = friend.GetComponent<FriendPrefab>();
        friendScript.friend = senderOrReceiver;

        if (isFriendList)
        {
            friendScript.SetAvailability(false, true, lobbyCodeToJoin);
        }
        else
        {
            // En este caso se trata de una solicitud
            var alreadyExistingRequest = FriendPrefab.pendingRequests.FirstOrDefault(nickname => nickname.Equals(senderOrReceiver.nickname));
            if (alreadyExistingRequest != null)
            {
                Destroy(friend);
                return;
            }

            friendScript.SetAvailability(true, false, lobbyCodeToJoin);
        }
    }
}

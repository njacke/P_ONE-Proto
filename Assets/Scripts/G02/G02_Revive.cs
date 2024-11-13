using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G02_Revive : MonoBehaviour
{
    public bool UseRevive(Vector3 targetPos, bool firstReviveUsed) {
        RaycastHit2D hit = Physics2D.Raycast(targetPos, Vector2.zero);

        if (hit.collider == null) {
            return false;
        }

        var npc = hit.collider.GetComponent<G02_NPC>();
        if (npc == null) {
            return false;
        }

        if (!firstReviveUsed && npc.CurrentNpcStatus == G02_NPC.NpcStatus.Hostile) {
            npc.CurrentNpcStatus = G02_NPC.NpcStatus.Friendly;
            Debug.Log("Blood sacrifice was used on " + npc.gameObject.name);
            return true;
        }
        
        if (npc.CurrentNpcStatus == G02_NPC.NpcStatus.Corpse) {
            npc.CurrentNpcStatus = G02_NPC.NpcStatus.Friendly;
            Debug.Log("Revive was used on " + npc.gameObject.name);
            return true;             
        }

        return false;
    }
}

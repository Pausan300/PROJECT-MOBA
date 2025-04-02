using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapatuEHealingArea : MonoBehaviour
{
    private bool m_ShowGizmo = false;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (m_ShowGizmo)
        {
            CapsuleCollider l_CapsuleCollider = GetComponent<CapsuleCollider>();
            if (l_CapsuleCollider != null)
            {
                Gizmos.color = new Color(0, 255, 0, 50);
                Gizmos.DrawMesh(Resources.GetBuiltinResource<Mesh>("Sphere.fbx"), transform.position + l_CapsuleCollider.center, transform.rotation, new Vector3(transform.localScale.x / 2, transform.localScale.x / 2, transform.localScale.x / 2));
            }
        }
    }
#endif
    public void Heal(float HealXSec)
    {
        StartCoroutine(ShowGizmoTemporarily());

        Collider l_Collider = GetComponent<Collider>();
        List<CharacterMaster> l_FoundCharacterMasters = new List<CharacterMaster>();
        Collider[] l_Colliders = Physics.OverlapBox(l_Collider.bounds.center, l_Collider.bounds.extents, l_Collider.transform.rotation);

        foreach (Collider Col in l_Colliders)
        {
            CharacterMaster l_Stats = Col.GetComponent<CharacterMaster>();
            if (l_Stats != null)
            {
                l_FoundCharacterMasters.Add(l_Stats);
            }
        }

        foreach (CharacterMaster _CharacterMaster in l_FoundCharacterMasters)
        {
            _CharacterMaster.AddHealth(HealXSec);
        }
        Debug.Log("Se han curado " + l_FoundCharacterMasters.Count + " aliados");
    }

    private IEnumerator ShowGizmoTemporarily()
    {
        m_ShowGizmo = true;
        yield return new WaitForSeconds(0.5f);
        m_ShowGizmo = false;
    }
}

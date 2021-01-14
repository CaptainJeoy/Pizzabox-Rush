using UnityEngine;

public abstract class SceneObject : MonoBehaviour
{
    public virtual void StoredDistance(int objID, Transform trans)
    {
        if (trans.position != null)
        {
            float newX = trans.position.x;

            /*
             * I commented out these funtions below because like i said in the video (make sure you watch the youtube video, link in the ReadMe), 
             * 'ToString("f6")' only formats the values for display purposes and doesn't change the fundamental decimal structure of the value.
             * it was just a mistake from my part
             * 
             * float newY = float.Parse((trans.position.y - GameManager.Instance.PizzaBoxControl.transform.position.y).ToString("f6"));
             * float newZ = float.Parse((trans.position.z - GameManager.Instance.PizzaBoxControl.transform.position.z).ToString("f6"));
             */

            //The amount of mantissa unity can accomodate with their single-precision float datatype is very enough for the purpose of the subraction below
            float newY = trans.position.y - GameManager.Instance.PizzaBoxControl.transform.position.y;
            float newZ = trans.position.z - GameManager.Instance.PizzaBoxControl.transform.position.z;

            GameManager.Instance.SceneManager.StoreDistance(objID, new Vector3(newX, newY, newZ));
        }
    }

    public virtual void ResetPosition(int objID, Transform trans)
    {
        Vector3 retrivedDist = GameManager.Instance.SceneManager.ReturnDistance(objID);
        trans.position = retrivedDist;

        GameManager.Instance.SceneManager.RemoveDistance(objID);
    }

    public virtual void RemovePosition(int objID)
    {
        GameManager.Instance.SceneManager.RemoveDistance(objID);
    }
}

//	Code is in base
public class Shovel : PickUpBase, IInteractor
{
    public bool CanInteractWith(IInteractable interactable)
    {
        switch (interactable)
        {
            case Plant _:
                return true;
            default:
                return false;
        }
    }

    public void OnInteract(IInteractable interactable)
    {

    }

    //	Start is called before the first frame update
    private void Start()
    {
    }

    //	Update is called once per frame
    private void Update()
    {
        //	TODO: What to do while digging?
    }
}

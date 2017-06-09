using Gaze;

public class TestCustomCondition : Gaze_AbstractConditions
{

    public void SatisfyCondition()
    {
        ValidateCustomCondition(true);
    }

    public void UnSatisfyCondition()
    {
        ValidateCustomCondition(false);
    }

}

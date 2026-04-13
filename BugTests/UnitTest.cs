using BugPro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BugTests;

[TestClass]
public class BugLifeCycleTests
{
    private Bug _testObject = null!;

    [TestInitialize]
    public void Init()
    {
        _testObject = new Bug();
    }

    [TestMethod]
    public void Test_Initial_State_Is_NewDefect()
    {
        Assert.AreEqual(Bug.State.NewDefect, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_Triage_Transition()
    {
        _testObject.StartTriage();
        Assert.AreEqual(Bug.State.Triage, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_Fix_Transition_Via_NoTime()
    {
        _testObject.StartTriage();
        _testObject.NoTime();
        Assert.AreEqual(Bug.State.Fix, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_Fix_Transition_Via_SeparateSolution()
    {
        _testObject.StartTriage();
        _testObject.NeedSeparateSolution();
        Assert.AreEqual(Bug.State.Fix, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_Closing_From_Triage()
    {
        _testObject.StartTriage();
        _testObject.OtherProduct();
        Assert.AreEqual(Bug.State.Closed, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_Ignore_NeedMoreInfo_In_Triage()
    {
        _testObject.StartTriage();
        _testObject.NeedMoreInfo();
        Assert.AreEqual(Bug.State.Triage, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_Successful_Fix_Completion()
    {
        _testObject.StartTriage();
        _testObject.NoTime();
        _testObject.FixDone();
        Assert.AreEqual(Bug.State.Closed, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_NotDefect_Closes_Bug()
    {
        _testObject.StartTriage();
        _testObject.NoTime();
        _testObject.NotDefect();
        Assert.AreEqual(Bug.State.Closed, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_DoNotFix_Closes_Bug()
    {
        _testObject.StartTriage();
        _testObject.NoTime();
        _testObject.DoNotFix();
        Assert.AreEqual(Bug.State.Closed, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_Duplicate_Closes_Bug()
    {
        _testObject.StartTriage();
        _testObject.NoTime();
        _testObject.Duplicate();
        Assert.AreEqual(Bug.State.Closed, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_NotReproducible_Closes_Bug()
    {
        _testObject.StartTriage();
        _testObject.NoTime();
        _testObject.NotReproducible();
        Assert.AreEqual(Bug.State.Closed, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_Reopen_From_Closed()
    {
        _testObject.StartTriage();
        _testObject.OtherProduct();
        _testObject.Reopen();
        Assert.AreEqual(Bug.State.Reopened, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_Reopen_From_Fix()
    {
        _testObject.StartTriage();
        _testObject.NoTime();
        _testObject.Reopen();
        Assert.AreEqual(Bug.State.Reopened, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_Restart_Triage_After_Reopen()
    {
        _testObject.StartTriage();
        _testObject.OtherProduct();
        _testObject.Reopen();
        _testObject.StartTriage();
        Assert.AreEqual(Bug.State.Triage, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_FixDone_Directly_After_Reopen()
    {
        _testObject.StartTriage();
        _testObject.NoTime();
        _testObject.Reopen();
        _testObject.FixDone();
        Assert.AreEqual(Bug.State.Closed, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_Invalid_Trigger_Throws_Exception()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _testObject.NoTime());
    }

    [TestMethod]
    public void Test_Invalid_FixDone_From_Triage()
    {
        _testObject.StartTriage();
        Assert.ThrowsException<InvalidOperationException>(() => _testObject.FixDone());
    }

    [TestMethod]
    public void Test_Invalid_NotDefect_From_Start()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _testObject.NotDefect());
    }

    [TestMethod]
    public void Test_Invalid_Reopen_From_Start()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _testObject.Reopen());
    }

    [TestMethod]
    public void Test_Multiple_Reopens_Stay_Reopened()
    {
        _testObject.StartTriage();
        _testObject.OtherProduct();
        _testObject.Reopen();
        _testObject.Reopen(); 
        Assert.AreEqual(Bug.State.Reopened, _testObject.CurrentState);
    }

    [TestMethod]
    public void Test_Ignore_VerifyOk_In_Fix()
    {
        _testObject.StartTriage();
        _testObject.NoTime();
        _testObject.VerifyOk();
        Assert.AreEqual(Bug.State.Fix, _testObject.CurrentState);
    }
}
namespace ConectaAtende.Domain.Services;

public interface ITriagePolicyService
{
    ITriagePolicy GetCurrentPolicy();
    void SetPolicy(string policyName);
    string GetCurrentPolicyName();
}

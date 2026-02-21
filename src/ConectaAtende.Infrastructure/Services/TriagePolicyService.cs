using ConectaAtende.Domain.Services;
using ConectaAtende.Infrastructure.Services.TriagePolicies;

namespace ConectaAtende.Infrastructure.Services;

public class TriagePolicyService : ITriagePolicyService
{
    private ITriagePolicy _currentPolicy;

    public TriagePolicyService()
    {
        _currentPolicy = new FirstComeFirstServedPolicy();
    }

    public ITriagePolicy GetCurrentPolicy()
    {
        return _currentPolicy;
    }

    public void SetPolicy(string policyName)
    {
        _currentPolicy = policyName.ToLower() switch
        {
            "firstcomefirstserved" or "fifo" => new FirstComeFirstServedPolicy(),
            "priority" => new PriorityPolicy(),
            "mixed" => new MixedPolicy(),
            _ => throw new ArgumentException($"Política '{policyName}' não encontrada")
        };
    }

    public string GetCurrentPolicyName()
    {
        return _currentPolicy.PolicyName;
    }
}

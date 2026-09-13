namespace SmartX.Shared.Models;

public static class DeploymentValidator
{
    // A node is valid only if it AND every descendant is configured.
    public static bool IsValidDeployment(DeploymentNode node)
    {
        if (!node.IsConfigured) return false;          // base case: this node fails
        if (node.Children.Count == 0) return true;      // base case: leaf, and it's fine

        foreach (var child in node.Children)
        {
            if (!IsValidDeployment(child)) return false; // recursive call
        }
        return true;
    }

    public static string? FindFirstInvalidPath(DeploymentNode node, string pathSoFar = "")
    {
        var currentPath = string.IsNullOrEmpty(pathSoFar) ? node.Name : $"{pathSoFar} -> {node.Name}";
        if (!node.IsConfigured) return currentPath;

        foreach (var child in node.Children)
        {
            var invalid = FindFirstInvalidPath(child, currentPath);
            if (invalid is not null) return invalid;
        }
        return null;
    }

    // Sample tree: Facility A -> Zone 1 -> Sub-Zone B, matching the brief's example.
    public static DeploymentNode BuildSeedTree() => new()
    {
        Name = "Facility A",
        IsConfigured = true,
        Children = new List<DeploymentNode>
        {
            new()
            {
                Name = "Zone 1", IsConfigured = true,
                Children = new List<DeploymentNode>
                {
                    new()
                    {
                        Name = "Sub-Zone B", IsConfigured = true,
                        Children = new List<DeploymentNode>
                        {
                            new() { Name = "ESP32-Node-14", IsConfigured = true },
                            new() { Name = "ESP32-Node-15", IsConfigured = true }
                        }
                    }
                }
            },
            new()
            {
                Name = "Zone 2", IsConfigured = true,
                Children = new List<DeploymentNode>
                {
                    new() { Name = "ESP32-Node-22", IsConfigured = false } // deliberately broken
                }
            }
        }
    };
}
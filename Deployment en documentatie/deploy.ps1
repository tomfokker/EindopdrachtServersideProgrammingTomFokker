Add-AzureRmAccount

Select-AzureRmSubscription -SubscriptionName "Azure for Students"

New-AzureRmResourceGroupDeployment -Name ExampleDeployment -ResourceGroupName TestResourceGroupTom -TemplateFile "DeploymentTemplate.json"
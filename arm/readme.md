Ce template, dans sa configuration par défaut, crée les ressources suivantes:

- Un serveur SQL avec les règles de firewall pour Sigmund
- Un elastic pool 50 DTU
- Un app service plan P1V2
- 3 environnements (prod, uat, qa) avec chacun:
  - Une BD SQL
  - Un storage account
  - Un app service avec une slot staging
  - Les configurations adéquates pour la connexion à la base de données et au storage account
  - Sur l'environnement de prod il y a:
    - Des backups pour la BD SQL
    - La géo-réplication pour le storage account
    - La configuration du serveur SMTP

Pour déployer le template:

- Adapter le fichier `mutuali-dedicated.parameters.json` ([doc](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/parameter-files))
  - Révisez la section `parameters` de `mutuali-dedicated.json` et configurez vos paramètres dans votre nouveau fichier.
  - Vous devez au minimum configurer les paramètres suivants:
    - `secret-seed`  
      Il s'agit d'une clé secrète arbitraire qui sera utilisée pour générer des clés HMAC et JWT pour chaque environnement.
    - `sql-admin-password`
  - Vous devriez probablement aussi configurer:
    - Le nom des ressources
    - Les infos SMTP pour la production
    - L'URL des environnements (faites-le directement dans la `defaultValue` du paramètre)  
      Ces URLs sont utilisés comme `BaseUrl` par défaut dans les courriels.
  - **Attention**: Idéalement, votre repository ne devrait pas contenir les secrets de production. Les paramètres de type `securestring` devraient donc être stockés à part. Une bonne solution pour ça est d'utiliser Azure Key Vault et de configurer des références aux secrets stockés dans le vault. [Voir la doc](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/key-vault-parameter?tabs=azure-cli%2Cjson).
- Assurez-vous d'avoir [installé Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- Exécutez ces commandes:

  ```bash
  # Se connecter à Azure (ouvre une fenêtre de browser)
  az login

  # Sélection de la Subscription Azure
  az account set -s <subscription-id-où-on-veut-déployer>

  # Créer le resource group (s'il n'existe pas encore)
  az group create -l canadacentral -n <nom-du-resource-group>

  # Déployer les ressources dans le groupe (peut être exécuté plusieurs fois pour mettre à jour les ressources)
  az deployment group create -g <nom-du-resource-group> -f mutuali-dedicated.json -p mutuali-dedicated.parameters.json
  ```

Le fichier `release-pipeline.json` peut être importé dans Azure DevOps pour créer la base de la configuration d'un release.

Essentiellement ça crée deux _Stages_:
- _Staging_
  - Stop la slot staging
  - Déploie le code sur la slot staging
  - Démarre la slot staging
- _Swap_
  - Swap la slot staging avec la production
  - Stop la slot staging

Après l'avoir importé assurez-vous de configurer les variables de votre release:
- `appService`  
  Le nom du App Service où vous déployez ce release. On assume que celui-ci contient une slot nommée `staging`.
- `resourceGroup`  
  Le nom du Resource Group qui contient le App Service.
- `subscription`  
  L'ID de la Service Connection dans Azure DevOps qui est connectée à la Subscription Azure où l'app est déployée.  
  Pour l'obtenir, allez dans les _Project settings_ de votre projet Azure DevOps, puis dans la section _Service connections_.
  Cliquez sur la connexion appropriée (ou créez la si elle n'existe pas encore), puis dans l'URL vous devriez retrouver un paramètre `resourceId=<guid>`.
  C'est ce GUID que vous devez copier. 

Vous devrez aussi configurer l'artéfact à utiliser pour le déploiement:
- Dans la section _Pipeline_, cliquez sur _Add an artifact_
- Sélectionnez votre pipeline CI
- Cliquez sur _Add_

Si vous souhaitez automatiser le déploiement à partir des builds d'une branche:
- Cliquez sur l'éclair à côté de l'artéfact
- Activez le _Continuous deployment trigger_
- Ajoutez un filtre sur la branche désirée

Si vous souhaitez ajouter une approbation avant le swap (utile pour la prod):
- Cliquez sur le bonhomme immédiatement à gauche de l'étape Swap
- Activez le _Pre-deployment approvals_
- Sélectionnez le ou les utilisateurs ou groupes autorisés à approuver la mise en ligne
# Gouvernance de Sécurité et Composants Critiques - Mutuali

Ce document définit la stratégie de sécurité des services et identifie les composants critiques du projet Mutuali, conformément aux principes d'intégrabilité (A 1.12) de Montréal en Commun.

## 1. Gouvernance de la sécurité au niveau des services

La sécurité des services de Mutuali repose sur une approche multicouche intégrée à l'infrastructure Azure :

* **Authentification & Autorisation :** L'accès aux services s'effectue via un login courriel avec authentification à deux facteurs (2FA) obligatoire par SMS (via Twilio). Les appels API sont sécurisés par des jetons porteurs (Bearer Tokens).
* **Chiffrement :** * **En transit :** Toutes les communications entre le frontend Vue.js, le backend .NET Core et les services tiers sont chiffrées via TLS (HTTPS).
    * **Au repos :** Les données dans Azure SQL Server sont chiffrées par défaut (Transparent Data Encryption).
* **Gestion des secrets :** Aucune clé secrète n'est présente dans le code source. Les chaînes de connexion et clés d'API sont stockées dans les **Variables d'environnement Azure** et les **Connection Strings** sécurisés du portail Azure.
* **Souveraineté des données :** Pour respecter les exigences de conformité locales, l'ensemble des données et des services est hébergé dans la région **Azure Canada Est (Québec)**.

## 2. Identification des composants critiques

| Composant | Rôle | Criticité | Mesures de protection et résilience |
| :--- | :--- | :--- | :--- |
| **Azure SQL Server** | Stockage des données & Mots de passe | **Haute** | Sauvegardes automatiques Azure, chiffrement des mots de passe (hashing), isolation réseau. |
| **Backend .NET Core** | Logique d'affaires et API | **Haute** | Surveillance via Azure Insights (recyclage auto si défaillance), monitoring StatusCake. |
| **Passerelle Twilio** | Envoi des codes 2FA | **Moyenne** | Dépendance critique pour l'accès utilisateur; gérée par politique de retry. |
| **Frontend Vue.js** | Interface utilisateur | **Moyenne** | Hébergement statique sécurisé, protection contre les attaques XSS. |

## 3. Monitoring et Disponibilité

La résilience du système est assurée par une boucle de rétroaction automatisée :
1.  **StatusCake :** Surveille le temps de disponibilité (uptime) et alerte l'équipe en cas d'interruption.
2.  **Azure Application Insights :** Analyse les performances en temps réel et redémarre les instances de service si des signes de dégradation ou de plantage sont détectés.
3.  **Continuité :** Les politiques de sauvegarde d'Azure garantissent une récupération rapide en cas de corruption de données.
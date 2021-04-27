# Songho-OrbitCamera
Ce projet est inspiré très fortement du site http://www.songho.ca/opengl et particulièrement du programme C++ : http://www.songho.ca/opengl/files/OrbitCamera.zip pour la partie dessin et le chargement du modèle. 
Il permet d'avoir un aperçu de la mise en œuvre de la librairie OpentTK pour le Net Core5.0 et du GLControl dans cet environement
Il se compose d'une partie qui concerne le dessin de la scène et d'un formulaire avec deux GLControls permettent d'afficher la scéne avec 2 point de vue différents.

## Actions possibles
   - Rendu des triangles en Plein, Fil, Point du coté Point de Vue -->  F
   - Bouton Gauche de la souris appuyé --> déplacement de la caméra sur les 3 axes coté Vue d'ensemble
   - Bouton Droit de la souris appuyé --> éloignement ou rapprochement du point de visée coté Vue d'ensemble (zoom)
   - Molette de la souris --> éloignement ou rapprochement du point de visée coté Vue d'ensemble (zoom)
   - 3 TrackBars permettent la modification des angles de la caméra du Point de Vue
   - 3 TrackBars permettent la modification de la position de la caméra du Point de Vue
   - 3 TrackBars permettent la modification de la position du point visé par la caméra d Point de Vue

## Particularité du code
La partie dessin utilise le mode OpenGL immédiat mais aussi les VertexArray. Nous ne somme pas encore dans l'OpenGL dit moderne c'est à dire la version 3.3 et supérieure.
Nous avons quand même la mise en place de buffers et des indices qui sont les précurseurs des VBOs. 
La class ObjModel permet la lecture des fichiers au format .Obj et la création des données nécessaires aux ArrayBuffer et aux ElementsArrayBuffer.
La class OrbitCamera permet de voir la caméra comme un objet ayant subi un déplacement dans le monde global



- Implémentation de l'application et du formulaire WindowsForms. 
   - Application démarre à partir d'une procédure Main qui lance le formulaire principal. Voir la configuration de l'Application dans la fenêtre des propriétés de la solution.
   - Le formulaire WindowsForms n'utilise pas le declarateur de variable `WithEvents` spécifique à VB mais ajoute explicitement les évenements du formulaire, de ces controles et le constructeur `New`. 
   Cela n'empêche pas l'utilisation du concepteur de formulaire. 
   - Les class ObjModel et OrbitCamera originales fournissent des fonctionnalités qui ne sont pas nécessaires pour ce programmes. Elles ont été traduites en VB mais pas testées. Elles ont été déportées dans le répertoire Surplus.
   - Le GLControl n'est pas disponible dans le concepteur de formulaire. Vous pouvez le remplacer par un control Panel afin d'obtenir les propriétés de mise en page que vous pourrez récupérer lors de la configuration du GLControl dans le code.
```visual basic.net
   'Ajout dans le New ou le Load du formulaire
   'création du control hors désigner
   Rendu3emePerson = New GLControl() With {
            .API = Common.ContextAPI.OpenGL,
            .APIVersion = New Version(3, 3, 0, 0),
            .Flags = Common.ContextFlags.Default,
            .IsEventDriven = True,
            .Profile = Common.ContextProfile.Compatability,
            .Name = "Rendu3emePerson",							'propriété de mise en page
            .Location = New Point(0, 0),						'propriété de mise en page
            .Size = TailleRendu}								'propriété de mise en page
   'ajout du controle sur le formulaire
   Controls.Add(RenduOpenGL)
```
- GLControl Core. 
	- Le package Nuget du GLControl Core n'est pas disponible. Il faut donc créer la dll correspondant en télechargeant le code ici : https://github.com/opentk/GLControl
	- Chaque GLControl doit avoir son contexte OpenGL. Les objets OpenGL, ArrayBuffer et ElementsArrayBuffer sont créés dans chaque contexte ce qui n'est pas le cas en framework 4.8 où les objets OpenGL sont communs

## Type de projet
- Projet VS 2017 Net Core 5.0
- Dépendance Librairie OpenTK version 4.6.3. Package NuGet à partir de la gestion des packages NuGet sous VS
- Dépendance Librairie OpenTK.GlControl Net Core 5.0. dll fournie à partir d'une compilation OpenTK version 4.6.3 et Net Core 5.0
- Dépendance Librairie OpenTK.Texte fournie à partir d'une compilation OpenTK version 4.6.3 et Net Core 5.0 ou utilisation du code VB (traduction C# --> VB)

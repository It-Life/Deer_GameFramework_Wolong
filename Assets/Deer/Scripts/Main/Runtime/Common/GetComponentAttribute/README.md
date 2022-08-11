# Get Component Attribute
A little attribute that makes Awake/Start GetComponent calls redundant.

Generally in unity, we need to fill fields like this as follows:
```c#
private Rigidbody rigidbody;
void Start(){
    rigidbody = GetComponent<Rigidbody>();
}
```

But especially while working with teams, if you DI through Scene objects from inspector,
Scene merge conflicts become inevitable. But we shouldn't dirty our code base with those get component methods in various fields.
It will reduce readability.
**This plugin, removes this requirement and takes care of it with an attribute in background before scene begins.**

## Example Usage
```c#
[GetComponent] //Get from same gameobject
private Rigidbody rigidbody;

[GetComponent(GetComponentFrom.SceneObject)] //Scan the scene and get from first object.
private Rigidbody rigidbody;

[GetComponent(GetComponentFrom.TargetGameObject,"MyAwesomeTargetObjectName")] //Get from a target object at scene
private Rigidbody rigidbody;
```

## Installation
Just clone the repo and copy the folder into your project.

## Note
**[!]GetComponentFrom.TargetGameObject currently runs between awake and start methods.**

## TO-DO
- Editor time component fetching
- On-demand(non awake based) runtime component fetching
- Fetch signals(Event driven fetch on runtime)

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
[MIT](https://choosealicense.com/licenses/mit/)


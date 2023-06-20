# Unity Coroutine with Generic Result

Provides a replacement for Unity game engine coroutines that can return a generic result, mostly used in a similar manner to `UnityEngine.Coroutine`.

## Usage

To use the `Coroutine<T>` class, follow these steps:

1. Ensure that you have a reference to a `MonoBehaviour` instance.
2. Call the `StartCoroutine<T>` extension method on your MonoBehaviour instance. (Note that `this.` is required to use an extension method in C#.)
3. Pass the routine you want to execute as an `IEnumerator` parameter.
4. Cast the `Coroutine<T>` to a `UnityEngine.Coroutine` to wait for execution.
5. Access the `.Result`.

```csharp
using System;
using System.Collections;
using UnityEngine;

public class CoroutineExample : MonoBehaviour
{ 
    private IEnumerator Start()
    {
        // Start the coroutine
        Coroutine<int> coroutine = this.StartCoroutine<int>(YourRoutine());

        // Wait for the coroutine to complete by casting to a UnityEngine.Coroutine
        yield return (Coroutine)coroutine;

        try
        {
            // Access the result of the coroutine
            int result = coroutine.Result;
            Debug.Log("Coroutine result: " + result);
        }
        catch (ApplicationException exception)
        {
            // Handle any excpetion
            Debug.LogError("Coroutine exception: " + exception.Message);
        }
    }

    private IEnumerator YourRoutine()
    {
        // Your coroutine logic here
        yield return new WaitForSeconds(2);

        // Exceptions will be re-thrown
        if (UnityEngine.Random.value < 0.5f)
        {
            throw new ApplicationException("Something went wrong in the coroutine!");
        }

        yield return 42;
    }
}
```

In the example above, a coroutine `YourRoutine` is started using the `StartCoroutine<int>` extension method. The result of the coroutine is accessed using the `Result` property of the `Coroutine<int>` object.

### Exceptions

Exception will be thrown if any of these circumstances are true:
- Attempting to access the result of a coroutine before it has yielded a result will throw an `InvalidOperationException`.
- After a coroutine is cancelled with `.Cancel()`, attempting to access the result will throw an `OperationCanceledException`.
- If the coroutine throws an exception during execution, the `Coroutine<T>` object will capture and re-throw the exception when the result is accessed.

## API Reference

### `MonoBehaviourExtension`

A static class that extends the MonoBehaviour class with the `StartCoroutine<T>` extension method.

#### Methods

- `StartCoroutine<T>(this MonoBehaviour monoBehaviour, IEnumerator routine)`: Starts a coroutine and returns a `Coroutine<T>` object.

### `Coroutine<T>`

A class that represents a coroutine with a result.

#### Properties

- `Result`: The result of the coroutine. Accessing this property will throw an exception if the coroutine has not yielded a result yet.
- `Cancel()`: Cancels the execution of the coroutine.
- `implicit operator UnityEngine.Coroutine`: Allows implicit conversion from `Coroutine<T>` to `UnityEngine.Coroutine`.

## License

This code is provided under the [MIT License](https://opensource.org/licenses/MIT). Feel free to modify and use it in your projects.

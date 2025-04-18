# InterfaceSerializer for Unity

## Description
InterfaceSerializer is a lightweight Unity editor tool that enables serialization of interface references in the Inspector. It solves the common Unity limitation where interfaces cannot be directly assigned in the Editor.

## Key Features
🛠️ Universal Interface Support - Works with any C# interface

🔍 Type-Safe Validation - Prevents incorrect assignments

⚡ Zero Runtime Overhead - All processing happens in Editor

📦 UPM-Compatible - Easy installation via Git URL

## Installation
Add to your project via Unity Package Manager:

Open Window > Package Manager

Click + > Add package from Git URL

Paste:
https://github.com/VodVas/InterfaceSerializer/edit/main/README.md)](https://github.com/VodVas/InterfaceSerializer.git?path=/Assets/Plugins/InterfaceSerializer

## Usage
Create your interface:

```csharp
public interface IDamageable { void TakeDamage(float amount); }
```
Implement in MonoBehaviour:

```csharp
public class Enemy : MonoBehaviour, IDamageable { ... }
```
Reference in other components:

```csharp
[SerializeField, InterfaceConstraint(typeof(IDamageable))] 
private MonoBehaviour _damageable;
```
```csharp
//Access the interface//
private IDamageable Damageable => _damageable as IDamageable;
```

## Requirements
Unity 2019.4+ (LTS recommended)

## No external dependencies

## Technical Details
Editor-Only Processing - No runtime performance impact

Automatic Validation - Immediate feedback for incorrect assignments

Clean Architecture - SOLID-compliant implementation




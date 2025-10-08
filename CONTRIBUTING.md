# Contributing to Pac-Man Motion Game

Thank you for your interest in contributing! This document provides guidelines for contributing to the project.

## 🎯 Current Phase

We are currently in **Phase 1** - Building the core game mechanics.

**Completed:**
- ✅ P1-01: Map and Pellet System

**In Progress / Coming Soon:**
- ⏳ P1-02: Player Controller and Movement
- ⏳ P1-03: Camera Integration
- ⏳ P1-04: Pose Detection
- ⏳ P1-05: Ghost AI
- ⏳ P1-06: UI System
- ⏳ P1-07: Audio and Effects
- ⏳ P1-08: Mobile Build
- ⏳ P1-09: Performance Optimization

## 🚀 How to Contribute

### 1. Fork and Clone
```bash
# Fork the repository on GitHub, then:
git clone https://github.com/YOUR_USERNAME/Pac-Man-Motion-Game.git
cd Pac-Man-Motion-Game
```

### 2. Create a Branch
```bash
# Create a feature branch
git checkout -b feature/your-feature-name

# Or for bug fixes
git checkout -b fix/bug-description
```

### 3. Make Your Changes

Follow these guidelines:

#### Code Style
- Use C# naming conventions
- Follow Unity best practices
- Add XML comments for public methods
- Keep functions small and focused
- Use meaningful variable names

Example:
```csharp
/// <summary>
/// Spawns a pellet at the specified position
/// </summary>
/// <param name="position">World position for the pellet</param>
/// <param name="isPowerPellet">True if this is a power pellet</param>
/// <returns>The spawned pellet GameObject</returns>
public GameObject SpawnPellet(Vector3 position, bool isPowerPellet = false)
{
    // Implementation
}
```

#### Project Structure
- Scripts go in `Assets/Scripts/`
- Scenes go in `Assets/Scenes/`
- Prefabs go in `Assets/Prefabs/`
- Sprites/textures go in `Assets/Sprites/`
- Audio goes in `Assets/Audio/`

#### Unity-Specific Guidelines
- Use SerializeField instead of public for Inspector fields
- Always null-check before using FindObjectOfType
- Use proper Unity component lifecycle (Awake, Start, Update)
- Avoid using Find() or FindGameObjectWithTag() in Update()
- Implement proper cleanup in OnDestroy()

### 4. Test Your Changes

Before submitting:
- [ ] Test in Unity Editor
- [ ] Check for Console errors
- [ ] Verify existing features still work
- [ ] Add comments explaining complex logic
- [ ] Update documentation if needed

### 5. Commit Your Changes

```bash
# Add your changes
git add .

# Commit with a clear message
git commit -m "Add feature: description of your changes"

# Push to your fork
git push origin feature/your-feature-name
```

#### Commit Message Format
```
Type: Short description

Detailed explanation of what and why (optional)

- Change 1
- Change 2
- Change 3
```

**Types:**
- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation changes
- `style:` Code style changes (formatting)
- `refactor:` Code refactoring
- `test:` Adding tests
- `chore:` Maintenance tasks

Examples:
```
feat: Add player movement controller

- Implement 4-direction movement
- Add collision detection with walls
- Smooth movement with lerp
```

```
fix: Correct pellet collision detection

Previously pellets weren't being collected properly
due to missing trigger collider. Now fixed.
```

### 6. Submit a Pull Request

1. Go to your fork on GitHub
2. Click "Pull Request"
3. Select your branch
4. Write a clear description of your changes
5. Link any related issues

**PR Template:**
```markdown
## Description
Brief description of what this PR does

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Documentation update
- [ ] Refactoring

## Testing
- [ ] Tested in Unity Editor
- [ ] No console errors
- [ ] Existing features still work

## Screenshots (if applicable)
Add screenshots showing your changes

## Related Issues
Closes #123
```

## 📋 What to Work On

### Good First Issues
- Add visual effects to pellet collection
- Create different map layouts
- Add keyboard shortcuts for game controls
- Improve wall visual appearance
- Add debug visualization tools

### Priority Features
Check the GitHub Issues page for:
- Issues labeled `good first issue`
- Issues labeled `help wanted`
- Issues labeled `priority`

### Feature Requests
Have an idea? Open an issue first to discuss it!

## 🐛 Reporting Bugs

When reporting bugs, include:

1. **Description**: What happened?
2. **Expected**: What should happen?
3. **Steps to Reproduce**:
   - Step 1
   - Step 2
   - Step 3
4. **Unity Version**: e.g., 2021.3.0f1
5. **Platform**: Windows/Mac/Linux
6. **Screenshots/Logs**: If applicable

## 💡 Suggesting Features

When suggesting features:

1. **Use Case**: Why is this needed?
2. **Proposed Solution**: How should it work?
3. **Alternatives**: Other ways to solve this
4. **Additional Context**: Any other relevant info

## 🔍 Code Review Process

All PRs go through code review:

1. **Automated Checks**: CI/CD must pass
2. **Code Review**: At least one maintainer approves
3. **Testing**: Changes are tested
4. **Documentation**: Docs are updated if needed
5. **Merge**: Merged by maintainer

## 📚 Resources

### Learning Unity
- [Unity Learn](https://learn.unity.com/)
- [Unity Documentation](https://docs.unity3d.com/)
- [Brackeys YouTube](https://www.youtube.com/user/Brackeys)

### Pac-Man Game Design
- [Pac-Man Dossier](https://www.gamasutra.com/view/feature/3938/the_pacman_dossier.php)
- [Game Design Patterns](https://gameprogrammingpatterns.com/)

### Pose Detection
- [MediaPipe](https://google.github.io/mediapipe/)
- [TensorFlow Lite](https://www.tensorflow.org/lite)
- [MoveNet](https://www.tensorflow.org/hub/tutorials/movenet)

## 🤝 Code of Conduct

### Our Pledge
We are committed to making participation in this project a harassment-free experience for everyone.

### Standards
- Be respectful and inclusive
- Accept constructive criticism
- Focus on what's best for the project
- Show empathy towards others

### Enforcement
Unacceptable behavior should be reported to the project maintainers.

## 📝 License

By contributing, you agree that your contributions will be licensed under the MIT License.

## ❓ Questions?

- Open an issue with the `question` label
- Contact the maintainers
- Check existing documentation

## 🎉 Recognition

Contributors will be:
- Listed in the project README
- Mentioned in release notes
- Credited in the game (for significant contributions)

---

**Thank you for contributing to Pac-Man Motion Game!** 🎮

Every contribution, no matter how small, helps make this project better.

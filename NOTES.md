# SideScrollerGame_Maniacs - Development Notes

## Session 1 - January 17, 2026

### Completed
- [x] Project setup with MonoGame (mgdesktopgl template)
- [x] Player class with movement and gravity
- [x] Variable jump height (tap for short hop, hold for high jump)
- [x] Floating platforms with collision detection
- [x] Goomba-style enemies with patrol AI
- [x] Enemy stomp mechanics (bounce on head to kill)
- [x] Player death/reset on enemy side collision
- [x] Score tracking (internal, not displayed yet)
- [x] Git repo initialized and pushed to GitHub

### Current Controls
- **A / Left Arrow** - Move left
- **D / Right Arrow** - Move right
- **Space / W / Up Arrow** - Jump (hold for higher)
- **Escape** - Exit

### Next Session Ideas
- [ ] Camera scrolling for larger levels
- [ ] Score display (requires adding a font)
- [ ] Coins/collectibles
- [ ] Sound effects
- [ ] Sprite integration (buddy working on art)
- [ ] Tilemap system for level design
- [ ] More enemy types / better AI

### Tuning Values (Player.cs)
- `MoveSpeed = 200f`
- `Gravity = 800f`
- `JumpForce = -350f`
- `JumpCutMultiplier = 0.5f` (early release cuts velocity)
- `HoldJumpGravityMultiplier = 0.5f` (floatier while holding jump)

### Goal
Export to Android phones eventually - MonoGame supports this via the mgandroid template.

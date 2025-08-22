# Changelog

All notable changes to the Overlay Application will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Impact font as default text font
- Text outline effect with automatic contrasting color (black or white)
- Toggleable glow effect (enabled by default)
- Zoom animation where text grows from center to full size
- Pulsating text effect with size and glow variation (-pulse fast|medium|slow)
- Text size options: small (48pt), medium (72pt), large (96pt)
- Default settings: pink/purple text (#FF69B4), glow enabled, medium size, centered on primary display
- New CLI option: -size small|medium|large for text sizing
- Standalone executable build scripts

### Changed
- Improved text alignment to properly center both horizontally and vertically
- Enhanced outline effect with thicker 8-point outline instead of 4-point
- Refined grid coordinate system (0,0 = top-left)
- Improved display detection and positioning
- Enhanced argument parsing for CLI

### Fixed
- Text clipping issues
- Display positioning accuracy
- Timer management for text rotation
- Horizontal text alignment in canvas containers

## [1.0.0] - 2025-08-22

### Added
- Initial release of Overlay Application
- Basic overlay display functionality
- Simple CLI interface
- Single display support
- Basic text customization options

[Unreleased]: https://github.com/your-org/overlay-app/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/your-org/overlay-app/releases/tag/v1.0.0
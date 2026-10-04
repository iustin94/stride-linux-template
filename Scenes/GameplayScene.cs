using Stride.Core;
using Stride.Core.Mathematics;
using Stride.CommunityToolkit.Engine;
using Stride.CommunityToolkit.Games;
using Stride.CommunityToolkit.Rendering.ProceduralModels;
using Stride.Engine;
using Stride.Engine.Processors;
using Stride.Games;
using Stride.Rendering;
using StrideTemplate.Core;
using StrideTemplate.Input;
using StrideTemplate.UI;

namespace StrideTemplate.Scenes;

public class GameplayScene : IGameScene
{
    private readonly Game _game;
    private readonly InputMap _inputMap;
    private readonly GameState _gameState;
    private readonly Action _onGameOver;
    private readonly List<Entity> _entities = new();

    private Entity _player = null!;
    private readonly List<Entity> _aliens = new();
    private readonly List<Entity> _bullets = new();
    private readonly List<Entity> _alienBullets = new();

    private float _alienDirection = 1f;
    private float _alienSpeed = 3f;
    private float _alienStepDown = 0.8f;
    private float _shootCooldown;
    private float _alienShootTimer;
    private bool _ended;
    private Scene _rootScene = null!;
    private HUD? _hud;

    private const float LeftBound = -8f;
    private const float RightBound = 8f;
    private const float PlayerY = 0.5f;
    private const float PlayerSpeed = 10f;
    private const float BulletSpeed = 15f;
    private const float AlienBulletSpeed = 8f;
    private const float TopBound = 18f;

    private Material _playerMat = null!;
    private Material _alienMat = null!;
    private Material _bulletMat = null!;
    private Material _alienBulletMat = null!;

    private readonly Random _rng = new();

    public GameplayScene(Game game, InputMap inputMap, GameState gameState, Action onGameOver)
    {
        _game = game;
        _inputMap = inputMap;
        _gameState = gameState;
        _onGameOver = onGameOver;
    }

    public void Load(Scene rootScene, IServiceRegistry services)
    {
        _rootScene = rootScene;

        // Reposition the default camera to face the play area straight on
        var cameraEntity = rootScene.Entities.FirstOrDefault(e => e.Get<CameraComponent>() != null);
        if (cameraEntity != null)
        {
            cameraEntity.Transform.Position = new Vector3(0, 8f, 25f);
            cameraEntity.Transform.Rotation = Quaternion.Identity;
            var scripts = cameraEntity.GetAll<ScriptComponent>().ToList();
            foreach (var script in scripts)
                cameraEntity.Remove(script);
        }

        // Materials
        _playerMat = _game.CreateMaterial(new Color(0, 200, 80));
        _alienMat = _game.CreateMaterial(new Color(220, 40, 40));
        _bulletMat = _game.CreateFlatMaterial(new Color(255, 255, 80));
        _alienBulletMat = _game.CreateFlatMaterial(new Color(255, 100, 255));

        // Player
        _player = _game.Create3DPrimitive(PrimitiveModelType.Cube, new Primitive3DEntityOptions
        {
            EntityName = "Player",
            Material = _playerMat,
            Size = new Vector3(1.2f, 0.4f, 0.6f)
        });
        _player.Transform.Position = new Vector3(0, PlayerY, 0);
        _player.Scene = rootScene;
        _entities.Add(_player);

        // Alien grid: 8 columns x 4 rows
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var alien = _game.Create3DPrimitive(PrimitiveModelType.Cube, new Primitive3DEntityOptions
                {
                    EntityName = $"Alien_{row}_{col}",
                    Material = _alienMat,
                    Size = new Vector3(0.8f, 0.8f, 0.6f)
                });
                float x = LeftBound + 2f + col * 1.8f;
                float y = 10f + row * 1.5f;
                alien.Transform.Position = new Vector3(x, y, 0);
                alien.Scene = rootScene;
                _aliens.Add(alien);
                _entities.Add(alien);
            }
        }

        // HUD
        _hud = new HUD(_gameState);
        _hud.Show(rootScene);
    }

    public void Update(GameTime time)
    {
        if (_ended) return;

        var dt = (float)time.Elapsed.TotalSeconds;

        UpdatePlayer(dt);
        UpdateBullets(dt);
        UpdateAliens(dt);
        UpdateAlienBullets(dt);
        CheckCollisions();
        AlienShooting(dt);

        if (_aliens.Count == 0)
        {
            _ended = true;
            _gameState.TriggerVictory();
            _onGameOver();
        }

        if (_gameState.IsGameOver && !_ended)
        {
            _ended = true;
            _onGameOver();
        }
    }

    private void UpdatePlayer(float dt)
    {
        var input = _game.Input;
        var pos = _player.Transform.Position;

        if (_inputMap.IsDown(input, InputAction.MoveLeft))
            pos.X -= PlayerSpeed * dt;
        if (_inputMap.IsDown(input, InputAction.MoveRight))
            pos.X += PlayerSpeed * dt;

        pos.X = MathUtil.Clamp(pos.X, LeftBound, RightBound);
        _player.Transform.Position = pos;

        _shootCooldown -= dt;
        if (_inputMap.IsDown(input, InputAction.Shoot) && _shootCooldown <= 0)
        {
            SpawnBullet(pos);
            _shootCooldown = 0.25f;
        }
    }

    private void SpawnBullet(Vector3 origin)
    {
        var bullet = _game.Create3DPrimitive(PrimitiveModelType.Cube, new Primitive3DEntityOptions
        {
            EntityName = "Bullet",
            Material = _bulletMat,
            Size = new Vector3(0.15f, 0.5f, 0.15f)
        });
        bullet.Transform.Position = new Vector3(origin.X, origin.Y + 0.5f, 0);
        bullet.Scene = _rootScene;
        _bullets.Add(bullet);
        _entities.Add(bullet);
    }

    private void UpdateBullets(float dt)
    {
        for (int i = _bullets.Count - 1; i >= 0; i--)
        {
            var pos = _bullets[i].Transform.Position;
            pos.Y += BulletSpeed * dt;
            _bullets[i].Transform.Position = pos;

            if (pos.Y > TopBound)
            {
                RemoveEntity(_bullets[i]);
                _bullets.RemoveAt(i);
            }
        }
    }

    private void UpdateAliens(float dt)
    {
        if (_aliens.Count == 0) return;

        float minX = float.MaxValue, maxX = float.MinValue;
        foreach (var alien in _aliens)
        {
            var x = alien.Transform.Position.X;
            if (x < minX) minX = x;
            if (x > maxX) maxX = x;
        }

        bool shouldReverse = (maxX >= RightBound && _alienDirection > 0) ||
                             (minX <= LeftBound && _alienDirection < 0);

        if (shouldReverse)
        {
            _alienDirection = -_alienDirection;
            foreach (var alien in _aliens)
            {
                var pos = alien.Transform.Position;
                pos.Y -= _alienStepDown;
                alien.Transform.Position = pos;

                if (pos.Y <= PlayerY + 1f)
                {
                    _gameState.LoseLife();
                    _gameState.LoseLife();
                    _gameState.LoseLife(); // Force game over
                }
            }
        }

        foreach (var alien in _aliens)
        {
            var pos = alien.Transform.Position;
            pos.X += _alienSpeed * _alienDirection * dt;
            alien.Transform.Position = pos;
        }
    }

    private void AlienShooting(float dt)
    {
        if (_aliens.Count == 0) return;

        _alienShootTimer -= dt;
        if (_alienShootTimer <= 0)
        {
            _alienShootTimer = 0.8f + (float)_rng.NextDouble() * 1.2f;
            var shooter = _aliens[_rng.Next(_aliens.Count)];
            var pos = shooter.Transform.Position;

            var bullet = _game.Create3DPrimitive(PrimitiveModelType.Cube, new Primitive3DEntityOptions
            {
                EntityName = "AlienBullet",
                Material = _alienBulletMat,
                Size = new Vector3(0.15f, 0.4f, 0.15f)
            });
            bullet.Transform.Position = new Vector3(pos.X, pos.Y - 0.5f, 0);
            bullet.Scene = _rootScene;
            _alienBullets.Add(bullet);
            _entities.Add(bullet);
        }
    }

    private void UpdateAlienBullets(float dt)
    {
        for (int i = _alienBullets.Count - 1; i >= 0; i--)
        {
            var pos = _alienBullets[i].Transform.Position;
            pos.Y -= AlienBulletSpeed * dt;
            _alienBullets[i].Transform.Position = pos;

            if (pos.Y < -2f)
            {
                RemoveEntity(_alienBullets[i]);
                _alienBullets.RemoveAt(i);
                continue;
            }

            var playerPos = _player.Transform.Position;
            if (Math.Abs(pos.X - playerPos.X) < 0.7f && Math.Abs(pos.Y - playerPos.Y) < 0.5f)
            {
                RemoveEntity(_alienBullets[i]);
                _alienBullets.RemoveAt(i);
                _gameState.LoseLife();
                return;
            }
        }
    }

    private void CheckCollisions()
    {
        for (int b = _bullets.Count - 1; b >= 0; b--)
        {
            var bPos = _bullets[b].Transform.Position;
            bool hit = false;

            for (int a = _aliens.Count - 1; a >= 0; a--)
            {
                var aPos = _aliens[a].Transform.Position;
                if (Math.Abs(bPos.X - aPos.X) < 0.6f && Math.Abs(bPos.Y - aPos.Y) < 0.6f)
                {
                    RemoveEntity(_aliens[a]);
                    _aliens.RemoveAt(a);
                    _gameState.AddScore(10);
                    hit = true;

                    _alienSpeed = 3f + (32 - _aliens.Count) * 0.15f;
                    break;
                }
            }

            if (hit)
            {
                RemoveEntity(_bullets[b]);
                _bullets.RemoveAt(b);
            }
        }
    }

    private void RemoveEntity(Entity entity)
    {
        entity.Scene = null;
        _entities.Remove(entity);
    }

    public void Unload(Scene rootScene)
    {
        _hud?.Dispose();
        foreach (var entity in _entities)
            entity.Scene = null;
        _entities.Clear();
        _aliens.Clear();
        _bullets.Clear();
        _alienBullets.Clear();
    }
}

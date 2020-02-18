using System.Collections;
using GXPEngine.Core;
using MathfExtensions;

namespace GXPEngine
{
    public class HunterBulletManager : GameObject
    {
        private HunterBullet[] _bulletsPool;
        private int bulletPoolIndex;

        private Level _level;

        public HunterBulletManager(Level pLevel) : base(false)
        {
            _level = pLevel;
            _bulletsPool = new HunterBullet[10];

            for (int i = 0; i < _bulletsPool.Length; i++)
            {
                var bullet = new HunterBullet();
                _level.AddChild(bullet);
                bullet.SetActive(false);

                _bulletsPool[i] = bullet;
            }

            CoroutineManager.StartCoroutine(WaitForPlayerData(), this);
        }

        private IEnumerator WaitForPlayerData()
        {
            while (_level.PlayerHasSpeed == null)
            {
                yield return null;
            }

            for (int i = 0; i < _bulletsPool.Length; i++)
            {
                var bullet = _bulletsPool[i];

                bullet.Speed = _level.PlayerHasSpeed.MaxSpeed * 2f;
            }
        }

        public void SpawnBullet(float x, float y, Vector2 direction, HunterGameObject shooter)
        {
            var bullet = GetBulletFromPool();

            bullet.SetXY(x, y);
            bullet.SetScaleXY(1, 1);

            var dirNorm = direction.Normalized;
            bullet.rotation = (Mathf.Atan2(dirNorm.y, dirNorm.x)).RadToDegree();

            bullet.Range = shooter.ScanEnemyRange * 1.2f;

            bullet.LifeTimeCounter = 0;

            bullet.SetActive(true);

            bullet.Shooter = shooter;
        }

        public void DespawnBullet(HunterBullet bullet)
        {
            bullet.SetActive(false);
            bullet.Shooter = null;
        }

        private HunterBullet GetBulletFromPool()
        {
            var bullet = _bulletsPool[bulletPoolIndex];
            bulletPoolIndex = ++bulletPoolIndex % _bulletsPool.Length;

            return bullet;
        }
    }
}
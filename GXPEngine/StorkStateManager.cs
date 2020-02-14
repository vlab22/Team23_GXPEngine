using System;
using System.Collections;
using System.Linq;
using GXPEngine.Core;

namespace GXPEngine
{
    public class StorkStateManager : IColliderListener
    {
        private Stork _stork;

        private bool _inCollisionWithDeliveryPoint;

        public StorkStateManager(Stork pStork)
        {
            _stork = pStork;
            _stork.ColliderListener = this;
        }

        void IColliderListener.OnCollisionWith(GameObject go, GameObject other)
        {
            if (!_inCollisionWithDeliveryPoint && other is DeliveryPoint)
            {
                //Get to end

                //Drop the pizza to the center of the delivery point
                var dropPoint = new Vector2(other.x, other.y);
                CoroutineManager.StartCoroutine(DropPizzaRoutine(dropPoint));

                _inCollisionWithDeliveryPoint = true;
            }
        }

        private IEnumerator DropPizzaRoutine(Vector2 dropPoint)
        {
            yield return new WaitForMilliSeconds(400);

            Console.WriteLine($"{this}: droppoint: {dropPoint}");

            //Get pizza game object
            var pizza = _stork.GetChildren().FirstOrDefault(go => go is PizzaGameObject) as PizzaGameObject;

            if (pizza == null)
            {
                yield break;
            }

            var from = _stork.TransformPoint(pizza.x, pizza.y);
            var fromX = from.x;
            var fromY = from.y;
          
            pizza.parent = _stork.parent;

            int time = 0;

            var fromScaleX = pizza.scaleX;
            var fromScaleY = pizza.scaleY;

            while (time < 500)
            {
                pizza.x = Easing.Ease(Easing.Equation.Linear, time, fromX, dropPoint.x, 500);
                pizza.y = Easing.Ease(Easing.Equation.Linear, time, fromY, dropPoint.y, 500);

                var scaleX = Easing.Ease(Easing.Equation.Linear, time, fromScaleX, 0, 500);
                var scaleY = Easing.Ease(Easing.Equation.Linear, time, fromScaleY, 0, 500);

                pizza.SetScaleXY(scaleX, scaleY);

                yield return null;
                time += Time.deltaTime;
            }
        }
    }
}
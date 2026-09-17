import { useEffect, useState } from 'react'
import './App.css'

const API_URL = 'http://localhost:5000/api/products'

function App() {
  const [products, setProducts] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    const loadProducts = async () => {
      try {
        const response = await fetch(API_URL)

        if (!response.ok) {
          throw new Error(`Request failed with status ${response.status}`)
        }

        const data = await response.json()
        setProducts(data)
      } catch (err) {
        setError(err.message || 'Unable to load products from the API.')
      } finally {
        setLoading(false)
      }
    }

    loadProducts()
  }, [])

  return (
    <div className="app-shell">
      <header className="topbar">
        <div>
          <p className="eyebrow">Local harvest</p>
          <h1>FarmToTable</h1>
        </div>
        <button type="button" className="primary-btn">
          Shop now
        </button>
      </header>

      <main className="catalog">
        <section className="hero-panel">
          <div>
            <p className="eyebrow accent">Fresh from the farm</p>
            <h2>Organic produce, delivered with care.</h2>
          </div>
          <p>
            Discover seasonal vegetables, fruit, dairy and pantry essentials sourced
            from trusted local growers.
          </p>
        </section>

        {loading && <div className="status">Loading products...</div>}
        {error && <div className="status error">{error}</div>}

        {!loading && !error && products.length === 0 && (
          <div className="status">No products are available right now.</div>
        )}

        <div className="product-grid">
          {products.map((product) => (
            <article key={product.id} className="product-card">
              <img
                src={product.imageUrl || 'https://images.unsplash.com/photo-1542838132-92c53300491e?auto=format&fit=crop&w=700&q=80'}
                alt={product.name}
                onError={(event) => {
                  event.target.src =
                    'https://images.unsplash.com/photo-1542838132-92c53300491e?auto=format&fit=crop&w=700&q=80'
                }}
              />

              <div className="card-body">
                <div className="meta-row">
                  <span>{product.category?.name || 'Farm fresh'}</span>
                  <span>{product.isOrganic ? 'Organic' : 'Seasonal'}</span>
                </div>

                <h3>{product.name}</h3>
                <p className="description">
                  {product.description || 'Freshly harvested and packed for your table.'}
                </p>

                <div className="details-row">
                  <span>{product.availableQuantity ?? 0} in stock</span>
                  <span>{product.deliveryTime || '2-3 days'}</span>
                </div>

                <div className="price-row">
                  <strong>R{Number(product.price || 0).toFixed(2)}</strong>
                  <button type="button">Add to basket</button>
                </div>
              </div>
            </article>
          ))}
        </div>
      </main>
    </div>
  )
}

export default App

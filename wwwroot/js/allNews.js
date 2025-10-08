const { useState, useEffect } = React;

function AllNews({ initialData }) {
    const [articles, setArticles] = useState([]);
    const [currentIndex, setCurrentIndex] = useState(0);
    const batchSize = 6;

    const loadMore = () => {
        const nextIndex = currentIndex + batchSize;
        const slice = initialData.slice(currentIndex, nextIndex);
        setArticles(prev => [...prev, ...slice]);
        setCurrentIndex(nextIndex);
    };

    useEffect(() => {
        loadMore();

        const onScroll = () => {
            if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 200) {
                if (currentIndex < initialData.length) loadMore();
            }
        };

        window.addEventListener("scroll", onScroll);
        return () => window.removeEventListener("scroll", onScroll);
    }, [currentIndex]);

    return (
        <div className="row">
            {articles.map(article => (
                <div key={article.id} className="col-md-4 mb-4">
                    <div className="card h-100 bg-dark text-light shadow-lg border-0">
                        {article.imagePath && <img src={article.imagePath} className="card-img-top" alt={article.title} />}
                        <div className="card-body">
                            <h5 className="card-title text-warning fw-bold">{article.title}</h5>
                            <h6 className="card-subtitle mb-3 text-info">{article.subtitle}</h6>
                            <p className="card-text text-light">{article.text.substring(0, 150)}...</p>
                            <a className="btn btn-gradient btn-lg w-100 fw-bold" href={`/News/Details/${article.id}`}>
                                Read More
                            </a>
                        </div>
                    </div>
                </div>
            ))}
        </div>
    );
}

document.addEventListener("DOMContentLoaded", () => {
    const container = document.getElementById("news-root");
    const initialData = JSON.parse(container.dataset.articles);
    const root = ReactDOM.createRoot(container);
    root.render(<AllNews initialData={initialData} />);
});
